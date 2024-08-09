import discord
from discord.ext import commands
import asyncio
import os
from pathlib import Path
from shutil import which


SCRIPT_DIRECTORY = Path(__file__).parent
CONFIG_FILE = SCRIPT_DIRECTORY.parent / 'config.txt'


def load_config(config_file):
    config = {}
    with open(config_file, 'r') as file:
        for line in file:
            if '=' in line:
                key, value = line.strip().split('=', 1)
                config[key] = value.strip()
    return config


config = load_config(CONFIG_FILE)
BOT_TOKEN = config.get('DISCORD_BOT_TOKEN')

MP3_DIRECTORY = config.get('DOWNLOAD_DIRECTORY', str(Path.home() / 'Documents'))

FFMPEG_PATH = which("ffmpeg") or str(SCRIPT_DIRECTORY / 'ffmpeg.exe')

intents = discord.Intents.default()
intents.message_content = True

bot = commands.Bot(command_prefix='!', intents=intents)

song_queue = asyncio.Queue()
current_voice_client = None
skip_event = asyncio.Event()

@bot.event
async def on_ready():
    activity = discord.Activity(type=discord.ActivityType.listening, name="Nothing yet...")
    await bot.change_presence(activity=activity)
    print(f'We have logged in as {bot.user}')

async def update_activity(song_name):
    activity = discord.Activity(type=discord.ActivityType.listening, name=song_name)
    await bot.change_presence(activity=activity)

@bot.event
async def on_message(message):
    await bot.process_commands(message)

    if message.content.startswith('!streaming'):
        parts = message.content.split(maxsplit=1)
        if len(parts) < 2:
            await message.channel.send('Please provide a filename after !streaming')
            return

        raw_filename = parts[1].strip()
        mp3_filename = raw_filename.strip('**').replace('||', '')
        mp3_file = Path(MP3_DIRECTORY) / mp3_filename

        if mp3_file.exists() and mp3_file.suffix.lower() == '.mp3':
            await song_queue.put(str(mp3_file))
            await print_queue(message.channel)
            if not current_voice_client or not current_voice_client.is_playing():
                await play_next_song(message)
        else:
            await message.channel.send('Error: MP3 file not found or filename is incorrect')

@bot.command(name='skip')
async def skip(ctx):
    global skip_event
    if current_voice_client and current_voice_client.is_playing():
        skip_event.set() 
        await ctx.send("Skipping the current song...")
    else:
        await ctx.send("No song is currently playing.")

@bot.command(name='p', aliases=['play'])
async def play(ctx):
    global current_voice_client
    voice_channel = ctx.author.voice.channel

    if not voice_channel:
        await ctx.send("You are not connected to a voice channel")
        return

    if current_voice_client and current_voice_client.channel != voice_channel:
        await current_voice_client.disconnect()
        current_voice_client = None

    if not current_voice_client:
        current_voice_client = await voice_channel.connect()
        await ctx.send(f"Joined {voice_channel}")

    if not current_voice_client.is_playing() and not song_queue.empty():
        await play_next_song(ctx)

async def play_next_song(message):
    global current_voice_client, skip_event
    skip_event.clear() 

    if not song_queue.empty():
        mp3_file = await song_queue.get()
        song_name = os.path.basename(mp3_file)

        await update_activity(song_name)

        try:
            voice_channel = message.author.voice.channel
            if not voice_channel:
                await message.channel.send("You are not connected to a voice channel")
                return

            if current_voice_client:
                if current_voice_client.channel != voice_channel:
                    await current_voice_client.disconnect()
                    current_voice_client = None

            if not current_voice_client:
                current_voice_client = await voice_channel.connect()

            def after_playing(error):
                nonlocal message
                if error:
                    print('Player error: %s' % error)
                else:
                    bot.loop.create_task(delayed_delete(mp3_file))
                if not skip_event.is_set():
                    bot.loop.create_task(play_next_song(message))

            current_voice_client.play(
                discord.FFmpegPCMAudio(mp3_file, executable=FFMPEG_PATH), after=after_playing)

            while current_voice_client.is_playing() or skip_event.is_set():
                if skip_event.is_set():
                    skip_event.clear()
                    current_voice_client.stop() 
                    await asyncio.sleep(1)  
                    break
                await asyncio.sleep(1)

        except Exception as e:
            print(f"Error playing MP3: {e}")
            await message.channel.send("An error occurred while playing the MP3")
            current_voice_client = None
            await play_next_song(message)

async def delayed_delete(mp3_file):
    await asyncio.sleep(1)  
    try:
        os.remove(mp3_file)
        print(f"Deleted {mp3_file}")
    except Exception as e:
        print(f"Error deleting file: {e}")

async def print_queue(channel):
    if song_queue.empty():
        await channel.send("The queue is currently empty.")
    else:
        queue_list = []
        queue_copy = song_queue._queue.copy() 
        for item in queue_copy:
            queue_list.append(os.path.basename(item))

def get_latest_mp3_file(directory):
    mp3_files = list(Path(directory).glob('*.mp3'))
    if not mp3_files:
        return None

    latest_file = max(mp3_files, key=lambda f: os.path.getctime(f))
    return str(latest_file)

bot.run(BOT_TOKEN)

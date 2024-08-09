# Multi-Language Discord Music Bot

#### Overview
This project is a multi-language Discord music bot that integrates a C# WinForms application with a Python script. The bot is designed to provide a user-friendly interface for managing and playing audio files on Discord. It allows users to download audio from YouTube, queue songs, and control playback in a Discord voice channel. The combination of a C# WinForms application for the graphical interface and a Python script for audio streaming demonstrates an innovative approach to building a robust and interactive bot.

### C# WinForms Application

#### Description
The C# WinForms application serves as the primary user interface for interacting with the Discord bot. It simplifies various tasks for the user, including:

- Entering YouTube URLs or search queries to download audio files.
- Selecting the directory where MP3 files will be saved.
- Configuring settings related to the Python script used for audio playback.
- Displaying logs and progress related to audio downloads.

#### Key Components
- **Form1.cs**: This is the main form of the application. It includes:
  - Text fields and buttons for user input and interaction.
  - Event handlers that process user actions such as downloading audio and starting the Python script.
  - Integration with the `yt-dlp` tool for downloading audio files and managing file paths.
- **Form1.Designer.cs**: Contains auto-generated code that defines the layout and design of the form.
- **config.txt**: A configuration file used to store essential settings, including:
  - The Discord bot token.
  - Paths to the download directory and the Python script.
- **AudioModule**: A directory that includes the necessary executables:
  - `yt-dlp.exe` for downloading audio from YouTube.
  - `ffmpeg.exe` for processing and converting audio files.
  - As well as the `AudioModule.py` script and its dependencies.

#### Design Choices
Initially, the plan was to develop the entire bot using C# due to extensive experience with WinForms and Discord bot development in C#. However, integrating audio playback in C# proved challenging due to limited documentation and support for this specific task. After exploring alternatives, it became clear that Python, with its robust libraries and extensive documentation for Discord audio streaming, would be a more suitable choice.

Combining C# for the user interface with Python for audio management presented a unique challenge: integrating two different programming languages within a single project. This approach allowed leveraging the strengths of both languages: C# for a straightforward and intuitive graphical interface, and Python for its superior support for audio streaming. This decision was driven by both practical needs and an interest in tackling the complexity of multi-language integration.

### Python Discord Bot Script

#### Description
The Python script manages the audio playback within Discord. It handles:

- Connecting to voice channels and playing MP3 files.
- Maintaining a queue of songs and managing playback.
- Updating the bot’s presence to reflect the currently playing song.
- Handling commands for skipping tracks and managing the playlist.

#### Key Components
- **AudioModule.py**: This script is the core of the Discord bot. It performs several functions:
  - Connects to Discord and joins voice channels.
  - Plays MP3 files using `ffmpeg`.
  - Manages a queue of songs and handles user commands to skip or play songs.
  - Updates the bot's activity status to show the currently playing song.

#### Dependencies:
- `discord.py`: A Python library for interacting with the Discord API.
- `ffmpeg`: A multimedia framework used for processing audio files.

#### Design Choices
Using Python for the audio streaming component was influenced by its extensive support and well-documented libraries. Despite considering Java, Python was chosen due to familiarity and better support for Discord audio streaming tasks. The decision to integrate Python with the C# application was motivated by the desire to combine the best of both worlds: a user-friendly C# interface and robust audio capabilities in Python.

Integrating Python with C# presented challenges, particularly in ensuring smooth communication between the two components. This required developing creative solutions for inter-process communication and handling errors. Despite these challenges, the approach resulted in a powerful and versatile bot capable of performing complex tasks efficiently.

#### Communication Between C# and Python Components
The C# WinForms application and the Python script communicate through Discord commands. Specifically, the C# application sends a !streaming command followed by the filename of a song that has been downloaded. The Python script listens for this !streaming command in the Discord server and uses the provided filename as an argument to locate and play the corresponding MP3 file.

This method allows the two components to work together seamlessly, with the C# application handling the user interface and download process, and the Python script managing the playback and audio streaming within Discord.

### Installation and Setup

#### Discord Developer Portal Setup
1. Go to the [Discord Developers Portal](https://discord.com/developers/applications).
2. Click on the **New Application** button.
3. Enter your desired bot name, and click **Create**.
4. In your new application, go to the **Bot** tab.
5. Check all three check boxes in **privileged gateway intents**:
   - Presence intent.
   - Server members intent.
   - Message content intent.
6. Click **Add Bot**, and confirm **Yes, do it!**.
7. Get your Bot Token with the **Copy** button.
8. Change your bot's **Public Bot** setting off so only you can invite it, save.
9. Go to the **OAuth2** tab, check the **bot** check box in the OAuth2Url section.
10. Below that, under **Bot permissions**, check the **administrator** checkbox.
11. Finally, copy the generated URL and paste it into a browser to invite the bot to your server.

#### C# WinForms Application
- Install .NET Core: Ensure that .NET Core and Python are installed on your system.
- Edit `config.txt` to include the Discord bot token, download directory path, and `Python.exe` path.

### Usage
1. **Run the C# Application**: Start the WinForms application to interact with the bot.
2. **Download Audio**: Enter YouTube URLs or search queries in the application to download MP3 files.
3. **Configure Python Script**: Set up the Python script and ensure it’s running to handle audio playback.
4. **Use Discord Commands**:
   - `!play` to join a voice channel and start playing queued songs.
   - `!streaming` to queue a new song for playback.

### Troubleshooting
- **Check Configuration**: Make sure that the `config.txt` file contains the correct bot token and directory paths.
- **Review Logs**: Check logs for any errors related to audio playback or communication issues between C# and Python components.

### Conclusion
The integration of C# for the user interface and Python for audio streaming highlights an innovative approach to building a Discord music bot. This project not only provides a functional and interactive bot but also demonstrates the feasibility and benefits of combining different programming languages to leverage their respective strengths. The multi-language setup presents unique challenges but also offers valuable learning experiences and creative problem-solving opportunities.

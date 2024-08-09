using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        string baseDirectory = Directory.GetCurrentDirectory();
        private string downloadDirectory;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider? _services;
        private HashSet<string> mp3Files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Process? pythonProcess;
        private string pythonDirectory;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.GuildVoiceStates
            });
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();

            this.FormClosing += Form1_FormClosing;

            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string username = GetUsernameFromUserProfilePath(userProfilePath);
            pythonDirectory = $"C:/Users/{username}/AppData/Local/Programs/Python/Python312";
            downloadDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            

            
            LoadSettingsFromConfig("config.txt");
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string input = urlTextBox.Text;

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a YouTube URL or a search query.");
                return;
            }

            string formatString = input.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? input : $"ytsearch:{input}";
            string queryUrl = formatString.Replace(" ", "+");

            await DownloadVideoAsync(queryUrl);
        }

        private async Task DownloadVideoAsync(string url)
        {
            await Task.Run(() =>
            {
                string outputFileName = Path.Combine(downloadDirectory, "%(title)s.%(ext)s.%(epoch)s");
                string ffmpegDir = baseDirectory + "/AudioModule";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "yt-dlp.exe",
                    Arguments = $"-x --audio-format mp3 --ffmpeg-location \"{ffmpegDir}\" -o \"{outputFileName}\" {url}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    
                    process.OutputDataReceived += (sender, e) => {
                        if (e.Data != null)
                        {
                            ParseOutput(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) => {
                        if (e.Data != null)
                        {
                            ParseOutput(e.Data);
                        }
                    };
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });
        }

        private void ParseOutput(string data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ParseOutput), data);
            }
            else
            {
                if (!string.IsNullOrEmpty(data))
                {
                    txtOutput.AppendText(data + Environment.NewLine);

                    var match = Regex.Match(data, @"(\d+(\.\d+)?)%");
                    if (match.Success)
                    {
                        int progress = (int)(double.Parse(match.Value.TrimEnd('%')));
                        progressBar.Value = Math.Min(progress, 100);
                    }
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = downloadDirectory;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    downloadDirectory = folderDialog.SelectedPath;
                    downloadDirTextBox.Text = downloadDirectory;

                    SaveSettingsToConfig("config.txt");
                }
            }
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {

            _client.Log += Log;
            await RegisterCommandsAsync();

            string? token = GetBotTokenFromConfig("config.txt");
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("Bot token is missing. Please check the config.txt file.");
                return;
            }
            if (string.IsNullOrEmpty(pythonDirectory))
            {
                string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string username = GetUsernameFromUserProfilePath(userProfilePath);
                pythonDirectory = $"C:/Users/{username}/AppData/Local/Programs/Python/Python312";
                UpdateTextBox(PyDirTextBox, pythonDirectory);
            }

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            DeleteMp3FilesFromDirectory(downloadDirectory);
            StartPythonScript(baseDirectory + "/AudioModule");

            await Task.Delay(-1);
        }

        private string? GetBotTokenFromConfig(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("DISCORD_BOT_TOKEN="))
                    {
                        return line.Substring("DISCORD_BOT_TOKEN=".Length).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error reading config file: " + ex.Message);
            }

            return null;
        }

        private string? GetDownloadDirectoryFromConfig(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("DOWNLOAD_DIRECTORY="))
                    {
                        return line.Substring("DOWNLOAD_DIRECTORY=".Length).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error reading config file: " + ex.Message);
            }

            return null;
        }
        private string? GetAudioModuleConfig(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("AUDIOMODULE_SCRIPT_PATH="))
                    {
                        return line.Substring("AUDIOMODULE_SCRIPT_PATH=".Length).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error reading config file: " + ex.Message);
            }

            return null;
        }
        private string? GetPythonDirectoryFromConfig(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("PYTHON_DIRECTORY="))
                    {
                        return line.Substring("PYTHON_DIRECTORY=".Length).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error reading config file: " + ex.Message);
            }

            return null;
        }

        private void LoadSettingsFromConfig(string filePath)
        {
            downloadDirectory = GetDownloadDirectoryFromConfig(filePath);
            if (string.IsNullOrEmpty(downloadDirectory))
            {
                downloadDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            if (string.IsNullOrEmpty(pythonDirectory))
            {
                string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string username = GetUsernameFromUserProfilePath(userProfilePath);
                pythonDirectory = $"C:/Users/{username}/AppData/Local/Programs/Python/Python312";
            }
            downloadDirTextBox.Text = downloadDirectory;

            pythonDirectory = GetPythonDirectoryFromConfig(filePath);
            PyDirTextBox.Text = pythonDirectory;
        }

        private void SaveSettingsToConfig(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    bool tokenWritten = false;
                    bool directoryWritten = false;
                    bool pythonDirWritten = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("DISCORD_BOT_TOKEN="))
                        {
                            writer.WriteLine(line);
                            tokenWritten = true;
                        }
                        else if (line.StartsWith("DOWNLOAD_DIRECTORY="))
                        {
                            writer.WriteLine($"DOWNLOAD_DIRECTORY={downloadDirectory}");
                            directoryWritten = true;
                        }
                        else if (line.StartsWith("PYTHON_DIRECTORY="))
                        {
                            writer.WriteLine($"PYTHON_DIRECTORY={pythonDirectory}");
                            pythonDirWritten = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }

                    if (!tokenWritten)
                    {
                        writer.WriteLine("DISCORD_BOT_TOKEN=your_bot_token_here");
                    }
                    if (!directoryWritten)
                    {
                        writer.WriteLine($"DOWNLOAD_DIRECTORY={downloadDirectory}");
                    }
                    if (!pythonDirWritten)
                    {
                        writer.WriteLine($"PYTHON_DIRECTORY={pythonDirectory}");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error writing to config file: " + ex.Message);
            }
        }

        private void DeleteMp3FilesFromDirectory(string directoryPath)
        {
            try
            {
                var mp3Files = Directory.GetFiles(directoryPath, "*.mp3");
                foreach (var file in mp3Files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog,"Error deleting mp3 files: " + ex.Message);
            }
        }

        private Task Log(LogMessage arg)
        {
            UpdateTextBox(discordLog,arg.ToString());
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message is not SocketUserMessage userMessage) return;
            if (message != null)
            {
                string lowerString = message.Content.ToLower();
                string? formattedString = lowerString.StartsWith("!play") ? lowerString.Substring(5) : lowerString.StartsWith("!p") ? lowerString.Substring(2) : null;

                if (string.IsNullOrEmpty(formattedString)) return;

                string queryUrl = formattedString.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? formattedString.Trim() : $"ytsearch:{formattedString.Replace(" ", "+")}";
                UpdateTextBox(discordLog, $"Message received from {message.Author}: {message.Content}");
                string updateList = UpdateMP3List(downloadDirectory, mp3Files);
                await DownloadVideoAsync(queryUrl);

                string songName = UpdateMP3List(downloadDirectory, mp3Files);
                int webmIndex = songName.IndexOf(".webm");

                if (webmIndex != -1)
                {
                    string modifiedSongName = songName.Insert(webmIndex, "||");
                    await context.Channel.SendMessageAsync($"!streaming **{modifiedSongName}||**");
                }
                else
                {
                    await context.Channel.SendMessageAsync($"!streaming **{songName}**");
                }
                
            }
            else
            {
                UpdateTextBox(discordLog, "Message was null");
            }
            
        }

        private string UpdateMP3List(string directoryPath, HashSet<string> mp3Files)
        {

            /* TODO simplify this
            This was orginally used for something other than what its used for now,
            and I had a bug I thought this was causing turns out it wasnt,
            but I didnt know that until i had already completely over complicated it. */
            if (!Directory.Exists(directoryPath))
            {
                UpdateTextBox(discordLog, "Directory does not exist.");
                return "Directory does not exist.";
            }

            var currentFiles = new HashSet<string>(Directory.GetFiles(directoryPath, "*.mp3"), StringComparer.OrdinalIgnoreCase);

            UpdateTextBox(discordLog, $"Current files: {string.Join(", ", currentFiles)}");
            UpdateTextBox(discordLog, $"MP3 files: {string.Join(", ", mp3Files)}");

            var removedFiles = mp3Files.Except(currentFiles).ToList();
            if (removedFiles.Count > 0)
            {
                foreach (var file in removedFiles)
                {
                    mp3Files.Remove(file);
                    UpdateTextBox(discordLog, $"File removed: {file}");
                }
            }
            else
            {
                UpdateTextBox(discordLog, "No files were removed.");
            }

            var newFiles = currentFiles.Except(mp3Files).ToList();
            foreach (var file in newFiles)
            {
                mp3Files.Add(file);
                UpdateTextBox(discordLog, $"New file added: {file}");
                return Path.GetFileName(file);
            }

            return "No new files found.";
        }

        private void StartPythonScript(string scriptPath)
        {
            try
            {
                string pythonExecutablePath = Path.Combine(pythonDirectory, "python.exe");

                if (!File.Exists(pythonExecutablePath))
                {
                    MessageBox.Show("Python executable not found. Please check the configuration.");
                    return;
                }

                string fileName = Path.Combine(scriptPath, "AudioModule.py");

                if (!File.Exists(fileName))
                {
                    MessageBox.Show("Python script not found. Please check the script path.");
                    return;
                }

                pythonProcess = new Process
                {
                    StartInfo = new ProcessStartInfo(pythonExecutablePath, fileName)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };


                pythonProcess.OutputDataReceived += (sender, e) => {
                    if (e.Data != null)
                    {
                        HandlePythonOutput(e.Data);
                    }
                };
                pythonProcess.ErrorDataReceived += (sender, e) => {
                    if (e.Data != null)
                    {
                        HandlePythonOutput(e.Data);
                    }
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                UpdateTextBox(discordLog, $"Error starting Python script: {ex.Message}");
            }
        }

        private void HandlePythonOutput(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                UpdateTextBox(discordLog, $"Python: {data}");
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                pythonProcess.Kill();
                pythonProcess.Dispose();
            }
        }

        private void btnPython_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = pythonDirectory;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    pythonDirectory = folderDialog.SelectedPath;

                    SaveSettingsToConfig("config.txt");
                    PyDirTextBox.Text = pythonDirectory;
                }
            }
        }
        private void UpdateTextBox(TextBox textBox, String text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => textBox.AppendText(text + "\r\n")));
            }
            else
            {
                textBox.AppendText(text + "\r\n");
            }
        }
        static string GetUsernameFromUserProfilePath(string userProfilePath)
        {
            var directoryInfo = new DirectoryInfo(userProfilePath);
            return directoryInfo.Name;
        }

    }
}
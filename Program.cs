using System.Text;

Console.OutputEncoding = Encoding.UTF8;

// Command line mode
if (args.Length > 0)
{
    if (await HandleCommandLineAsync(args))
    {
        return;
    }
}

// Interactive mode
while (true)
{
    ShowMenu();
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await ConvertFileToBase64InteractiveAsync();
            break;

        case "2":
            await ConvertBase64ToFileInteractiveAsync();
            break;

        case "0":
            return;

        default:
            WriteError("Unknown option. Try again.");
            break;
    }

    Console.WriteLine();
    WriteInfo("Press Enter to continue...");
    Console.ReadLine();
}

static void ShowMenu()
{
    Console.Clear();
    WriteTitle("=== Base64 Converter (.NET 9) ===");
    WriteInfo("1) Convert file -> Base64");
    WriteInfo("2) Convert Base64 -> file");
    WriteInfo("0) Exit");
    Console.WriteLine();
    Console.Write("Select option: ");
}

static async Task<bool> HandleCommandLineAsync(string[] args)
{
    switch (args[0])
    {
        case "--help":
        case "-h":
        case "/?":
            PrintHelp();
            return true;

        case "--file-to-base64":
            if (args.Length < 2)
            {
                WriteError("Missing input file path.");
                PrintHelp();
                return true;
            }

            var inputFile = args[1];
            string? outFile = null;
            var printToConsole = true;

            for (var i = 2; i < args.Length; i++)
            {
                if (args[i] == "--out" && i + 1 < args.Length)
                {
                    outFile = args[i + 1];
                    i++;
                }
                else if (args[i] == "--no-print")
                {
                    printToConsole = false;
                }
            }

            await ConvertFileToBase64CliAsync(inputFile, outFile, printToConsole);
            return true;

        case "--base64-to-file":
            if (args.Length < 3)
            {
                WriteError("Usage: --base64-to-file <inputBase64.txt> <outputFile>");
                return true;
            }

            var base64Input = args[1];
            var decodedOutput = args[2];
            await ConvertBase64FileToFileCliAsync(base64Input, decodedOutput);
            return true;

        default:
            WriteError("Unknown command line option.");
            PrintHelp();
            return true;
    }
}

static void PrintHelp()
{
    WriteTitle("Base64 Converter (.NET 9) - command line usage:");
    WriteInfo("  Base64Converter.exe --file-to-base64 <inputFile> [--out <outputFile>] [--no-print]");
    WriteInfo("      Converts a file to Base64.");
    WriteInfo("      --out <outputFile>   Save Base64 to this file.");
    WriteInfo("      --no-print           Do not print Base64 to console.");
    Console.WriteLine();
    WriteInfo("  Base64Converter.exe --base64-to-file <inputBase64File> <outputFile>");
    WriteInfo("      Decodes Base64 from text file to binary file.");
    Console.WriteLine();
    WriteInfo("  Base64Converter.exe --help");
    WriteInfo("      Show this help.");
    Console.WriteLine();
}

// ===== Interactive operations =====

static async Task ConvertFileToBase64InteractiveAsync()
{
    Console.Write("Enter file path: ");
    var path = ReadTrimmedPath();

    if (!File.Exists(path))
    {
        WriteError("File not found.");
        return;
    }

    try
    {
        var bytes = await File.ReadAllBytesAsync(path);
        var base64 = Convert.ToBase64String(bytes);

        Console.WriteLine();
        WriteInfo("Output options:");
        WriteInfo("1) Print Base64 to console");
        WriteInfo("2) Save Base64 to file");
        Console.Write("Select option: ");
        var outputChoice = Console.ReadLine();

        switch (outputChoice)
        {
            case "1":
                Console.WriteLine();
                WriteTitle("=== Base64 ===");
                Console.WriteLine(base64);
                WriteTitle("==============");
                break;

            case "2":
                Console.Write("Enter output file path for Base64 text: ");
                var outPath = ReadTrimmedPath();
                await File.WriteAllTextAsync(outPath, base64, Encoding.UTF8);
                WriteSuccess($"Base64 string saved to: {outPath}");
                break;

            default:
                WriteError("Unknown option. Nothing was saved.");
                break;
        }
    }
    catch (Exception ex)
    {
        WriteError("Error while converting file to Base64:");
        WriteError(ex.Message);
    }
}

static async Task ConvertBase64ToFileInteractiveAsync()
{
    WriteInfo("Input options:");
    WriteInfo("1) Paste Base64 manually (multi-line, finish with empty line)");
    WriteInfo("2) Read Base64 from file");
    Console.Write("Select option: ");
    var inputChoice = Console.ReadLine();

    string base64;

    switch (inputChoice)
    {
        case "1":
            base64 = ReadMultiLineBase64();
            break;

        case "2":
            Console.Write("Enter file path with Base64 text: ");
            var base64Path = ReadTrimmedPath();
            if (!File.Exists(base64Path))
            {
                WriteError("File not found.");
                return;
            }
            base64 = await File.ReadAllTextAsync(base64Path, Encoding.UTF8);
            break;

        default:
            WriteError("Unknown option.");
            return;
    }

    base64 = base64.Trim();

    Console.Write("Enter output file path (where to save decoded file): ");
    var outputPath = ReadTrimmedPath();

    try
    {
        var bytes = Convert.FromBase64String(base64);
        await File.WriteAllBytesAsync(outputPath, bytes);
        WriteSuccess($"File saved to: {outputPath}");
    }
    catch (FormatException)
    {
        WriteError("Invalid Base64 string. Conversion failed.");
    }
    catch (Exception ex)
    {
        WriteError("Error while converting Base64 to file:");
        WriteError(ex.Message);
    }
}

// ===== CLI operations =====

static async Task ConvertFileToBase64CliAsync(string inputFile, string? outputFile, bool printToConsole)
{
    if (!File.Exists(inputFile))
    {
        WriteError($"Input file not found: {inputFile}");
        return;
    }

    try
    {
        var bytes = await File.ReadAllBytesAsync(inputFile);
        var base64 = Convert.ToBase64String(bytes);

        if (printToConsole)
        {
            WriteTitle("=== Base64 ===");
            Console.WriteLine(base64);
            WriteTitle("==============");
        }

        if (!string.IsNullOrWhiteSpace(outputFile))
        {
            await File.WriteAllTextAsync(outputFile, base64, Encoding.UTF8);
            WriteSuccess($"Base64 string saved to: {outputFile}");
        }
    }
    catch (Exception ex)
    {
        WriteError("Error in --file-to-base64:");
        WriteError(ex.Message);
    }
}

static async Task ConvertBase64FileToFileCliAsync(string base64InputFile, string outputFile)
{
    if (!File.Exists(base64InputFile))
    {
        WriteError($"Base64 input file not found: {base64InputFile}");
        return;
    }

    try
    {
        var base64 = await File.ReadAllTextAsync(base64InputFile, Encoding.UTF8);
        var bytes = Convert.FromBase64String(base64.Trim());
        await File.WriteAllBytesAsync(outputFile, bytes);
        WriteSuccess($"File saved to: {outputFile}");
    }
    catch (FormatException)
    {
        WriteError("Invalid Base64 content in input file.");
    }
    catch (Exception ex)
    {
        WriteError("Error in --base64-to-file:");
        WriteError(ex.Message);
    }
}

// ===== Helpers =====

static string ReadMultiLineBase64()
{
    WriteInfo("Paste Base64 below. Use empty line to finish:");
    var sb = new StringBuilder();

    while (true)
    {
        var line = Console.ReadLine();
        if (string.IsNullOrEmpty(line))
        {
            break;
        }

        sb.Append(line.Trim());
    }

    return sb.ToString();
}

static string ReadTrimmedPath()
{
    var input = Console.ReadLine() ?? string.Empty;

    input = input.Trim();
    if (input.StartsWith("\"") && input.EndsWith("\"") && input.Length >= 2)
    {
        input = input.Substring(1, input.Length - 2);
    }

    return input.Trim();
}

static void WriteWithColor(string message, ConsoleColor color)
{
    var old = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ForegroundColor = old;
}

static void WriteTitle(string message) => WriteWithColor(message, ConsoleColor.Cyan);
static void WriteInfo(string message) => WriteWithColor(message, ConsoleColor.Gray);
static void WriteSuccess(string message) => WriteWithColor(message, ConsoleColor.Green);
static void WriteError(string message) => WriteWithColor(message, ConsoleColor.Red);

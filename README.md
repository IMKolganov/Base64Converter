# Base64 Converter (.NET 9)

A lightweight and powerful console tool for converting **files ↔ Base64**.  
Supports interactive mode, command line mode, multi-line Base64 input, and colorized output.

## 🚀 Features

- 🎨 Colorized output (info/success/error)
- 🧩 Multi-line Base64 input (end with an empty line)
- ⚡ Fast file → Base64 conversion
- 📥 Base64 → file decoding
- 🖥 Interactive menu
- 🛠 Command-line support
- ✂️ Drag & drop file paths
- 🔧 Modern .NET 9 (top-level statements, async IO)

## 📦 Installation

Build manually:

```bash
dotnet build -c Release
```

## 🖥 Interactive Mode

Run the application:

```bash
Base64Converter.exe
```

Menu options:

1. Convert **file → Base64**
2. Convert **Base64 → file**
3. Exit

### Multi-line Base64 input  
Choose option 2 → input method → **paste Base64 (multiple lines)** → press **Enter on an empty line** to finish.

## 🛠 Command Line Usage

### Convert file → Base64

```bash
Base64Converter.exe --file-to-base64 input.bin
```

Save to file:

```bash
Base64Converter.exe --file-to-base64 input.bin --out encoded.txt
```

Do NOT print to console:

```bash
Base64Converter.exe --file-to-base64 input.bin --out encoded.txt --no-print
```

### Convert Base64 (text file) → file

```bash
Base64Converter.exe --base64-to-file encoded.txt output.bin
```

### Show help

```bash
Base64Converter.exe --help
```

## 📘 Examples

Encode an image:

```bash
Base64Converter.exe --file-to-base64 photo.jpg --out photo.b64
```

Decode Base64 into a PDF:

```bash
Base64Converter.exe --base64-to-file document.b64 document.pdf
```

Quiet mode:

```bash
Base64Converter.exe --file-to-base64 input.zip --out base64.txt --no-print
```

## 🎨 Color Output

- Cyan — titles  
- Gray — info  
- Green — success  
- Red — errors  

## 🧩 Multi-Line Base64 Input

Supports:

```
line 1
line 2
line 3
<empty line>
```

## 🛡 Requirements

- .NET 9 SDK
- Windows / Linux / macOS

## 📄 License

MIT License.

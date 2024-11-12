# Cervo

Cervo is designed to be a high-performance, lightweight, cross-platform immediate UI framework that leverages ImGui for rendering. We provide a straightforward, user-friendly API for building immediate-mode UI applications, with customizable widgets ready out of the box—and it’s easy to add your own as well.

**Cervo is still heavily under early development and not ready for production use. [Can have a lot major changes during early development]**

## Supported Platforms
✅ - Supported<br>
🛠️ - Partially Supported / Under development<br>
❌ - Not Supported (but planned)

- 🛠️ Windows (🛠️x64, ❌arm64)
- ❌ Linux (❌️x64, ❌arm64)
- ❌ MacOS (❌arm64)

## Development

### Prerequisites
- .NET 8.0 SDK or later
- Modern .NET IDE (Visual Studio 2022, Rider, Visual Studio Code) [Recommended]
- CMake 3.20 or later
- C++17 compatible compiler (MSVC, GCC, Clang)

### Setup
1. Clone the repository
2. Open the repository root directory in your terminal
3. Make sure submodules are initialized and updated by running `git submodule update --init --recursive` in the repository root directory.
4. Run `./vendor/Mochi.DearImGui/generate.cmd` to generate the ImGui bindings for C#. (.sh for Linux)

Now you can open the solution in your IDE and start developing.

## License
Cervo is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Credits
- [Dear ImGui](https://github.com/ocornut/imgui) by Omar Cornut
- [Mochi.DearImGui](https://github.com/MochiLibraries/Mochi.DearImGui) by David Maas

## Contributing

We welcome contributions to Cervo. Currently, there is no contribution guide, but you can open an issue or a pull request if you want to contribute.
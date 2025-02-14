![i was never book smart im money smart](https://r2.e-z.host/f1ff83bd-5f1f-47f3-93bb-ad00e44cf263/egl14zgm.png)

A Friday Night Funkin' engine built with developers in mind, especially with the use of Godot Engine!

This engine is designed to be as robust as possible while still keeping it as non-complicated as possible towards developers looking forward to using this engine.


### [JOIN OUR DISCORD!](https://discord.gg/HMDFMM3ffu)


# Status

This engine is in a very Work-In-Progress state at the moment, so not everything may be functional. The moment we feel like this engine is stable enough to be used in actual fan games is when we will remove the W.I.P and bump up the version to v1.0.

# Developing on Rubicon

### Prerequisites

Please make sure you have the following on your device:

- [Godot Engine v4.4-beta2 with .NET support](https://godotengine.org/download/archive/4.4-beta2/) (Other versions not guaranteed to work)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [rcedit](https://github.com/electron/rcedit) (Windows exclusive, required for Windows exporting)

If you're developing on Rubicon, whether it's contributing to the engine or making your own fan-game, you should be aware that the way this engine handles tasks is nothing like how most other Friday Night Funkin' engines handle them, if the switch to Godot wasn't enough of an indicator. 

We plan to have an API Reference and a wiki to help developers better understand the engine more easily, so please bear with us! :pray:

### Recommended IDEs

We recommend using an IDE that has intelligent code completion, syntax highlighting, and integrates closely with Godot. We reccomend [JetBrains Rider](https://www.jetbrains.com/rider/) and [Visual Studio Code](https://code.visualstudio.com/) with the [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode) plugins installed. They're totally free to us (non-commercially for Rider)!

[Visual Studio](https://visualstudio.microsoft.com/vs/) is also a good choice, but that's for Windows only.

### Languages

This engine uses mainly C# for its main code base.

For actual scripting however, either GDScript or C# works, whatever tickles yer fancy! This engine is designed with that in mind, so keep a lookout for signals and events to connect to, since that's how you'll be working with the engine.

You could possibly work with GDExtension, though we won't provide any support for those ourselves. (Theoretically you could use Haxe! We wouldn't recommend it though, C# is very similar, if not better functionally than it.)

### Disclaimer

We ask you that you please follow proper C# formatting when developing with C# on the main code base, like for example: PascalCase names for public and protected properties, and _camelCase for private properties (with the underscore).

# Contributing

We would appreciate your guys' help so much! The main way to help us out would be to submit issues or pull requests. It would be a struggle if everyone split off to make their own version of Rubicon when we could all benefit from it!

# License

Rubicon Engine's code is licensed under the [Apache License, Version 2.0](https://opensource.org/license/apache-2-0). Please see [the license file](LICENSE) for more info. If you don't want to read all that, basically you can use our code as long as you credit us properly.

# Credits

## Developers

- [Binpuki](https://twitter.com/binpuki_)
- [rivenpaws](https://e-z.bio/nullobjectreference)
- [legole0](https://twitter.com/legole0)

## Special Credits

- [firubii](https://github.com/firubii/) - HoloFunk note system and chart format (the base for Rubicon's note system and chart format)
- [Yakuza / Like A Dragon series](https://en.wikipedia.org/wiki/Yakuza_(franchise)) - No joke, the way Dragon Engine games are structured heavily inspired how Rubicon's scripting system works

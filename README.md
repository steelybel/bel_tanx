# raygamecsharp - sample project

This is a sample C# project setup with [raylib][raylib] for Visual Studio 2017.
Raylib is a simple game programming framework that is designed to be friendly to
beginners. It is created by [Ramon Santamaria (@raysan5)][raysan].

The language binding is maintained at [ChrisDill/Raylib-cs][raylib-cs] and
is compatible with Raylib 2.0. Work towards Raylib 2.5 can be tracked in
[this issue](https://github.com/ChrisDill/Raylib-cs/issues/22).

It is primarily intended for use by students in the Game Programming course at
the Seattle Campus of the Academy of Interactive Entertainment.

This is written against .NET Core 2.0 and primarily supports Windows.
Adjustments may be needed for other platforms.

[raylib]:https://github.com/raysan5/raylib
[raysan]:https://github.com/raysan5

## Building

This project supports by **Visual Studio 2017** or newer.

Clone the repository and open the solution in Visual Studio. Both the solution
and project should already be configured and ready to start working with. To
test this, build and run the provided sample project.

![A screenshot of the included sample project](.github/raygame.png)

The sample project that is provided is the [basic window example][basicexample]
from raylib. Further examples can be found in its [repository][rayexample].
Examples are also available in C++-only on the [website][rayexamplesite].

You can review the [cheatsheet][raycheat] for the full range of functions made
available through raylib. Note that the identifiers for some methods may differ
for the C\# language binding we're using. A full list of all methods and types
made available via the [binding can be found in the Raylib-cs repository][raylib-cs-bindings].

[basicexample]:https://github.com/ChrisDill/Raylib-cs/blob/master/Examples/core/core_basic_window.cs
[rayexample]:https://github.com/ChrisDill/Raylib-cs/tree/master/Examples
[rayexamplesite]:https://www.raylib.com/examples.html
[raycheat]:https://www.raylib.com/cheatsheet/cheatsheet.html
[raylib-cs]:https://github.com/ChrisDill/Raylib-cs
[raylib-cs-bindings]:https://github.com/ChrisDill/Raylib-cs/blob/master/Bindings/Raylib.cs

## License

MIT License - Copyright (c) 2019 Academy of Interactive Entertainment

For more information, see the [license][lic] file.

Third party works are attributed under [thirdparty.md][3p].

[lic]:LICENSE.md
[3p]:THIRDPARTY.md
[raylib]:https://github.com/raysan5/raylib

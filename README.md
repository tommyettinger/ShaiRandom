# ShaiRandom

[*That which is ordained.*](https://en.wikipedia.org/wiki/Shai)

Welcome to the homepage for ShaiRandom, a random number generator library for modern .NET platforms.

## What Is It?

ShaiRandom provides high-performance non-cryptographic pseudo-random number generators for simulation, game development, or any other purposes.
It also has statistical distributions and various kinds of utility code relating to random numbers, including a weighted probability table and
special generators for testing that can record or reproduce a given sequence. Unary hashes are also provided in the `Mixers` class, which can
be useful to supplant random number generators in some cases commonly encountered in game development.

It really is high-performance! Benchmarking has been done at most stages of the project on the generators (though not the distributions), and
they generally outperform System.Random regardless of whether it uses the optimizations in .NET 6 or not.

Serialization is a major priority for ShaiRandom; all pseudo-random number generators can be serialized to a short string and later deserialized
from that string. This includes generators that have been wrapped (with a wrapper) to alter their behavior. `Serializer.Serialize()` and
`Serializer.Deserialize()` are most of what you need here.

## Obtaining It

See [ShaiRandom on NuGet](https://www.nuget.org/packages/ShaiRandom/) for more.

## Licensing

### ShaiRandom
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

### Other Licenses
Licenses for other projects which ShaiRandom depends on or from which inspiration was taken are listed in the credits section.

## Credits
ShaiRandom depends on some other .NET Standard libraries for some of its functionality.  Those projects and their licenses are listed below.

### juniper
This Java random number generation library (by the same author as this library) has the same general structure for its random number generators and uses the same algorithms.
It is licensed under Apache 2, but permission from all contributors (or the only contributor, me) to the random number generators allows that code to be freely relicensed as MIT here.
The serialized forms of any generators present in both libraries tend to be either very similar or identical, which should help interoperability. Similarly, the results of most (but
not all) methods are the same if the states and algorithms are the same. The code in juniper was pulled out of [jdkgdxds](https://github.com/tommyettinger/jdkgdxds), which this README
mentioned in earlier revisions.
- [juniper](https://github.com/tommyettinger/juniper)
- [juniper License](https://github.com/tommyettinger/juniper/blob/main/LICENSE)

### GoRogue
GoRogue was used as an optimal project structure for a .NET library targeting modern versions, and ShaiRandom is meant to be usable by GoRogue.
Significant amounts of code have been exchanged back and forth to avoid duplication between ShaiRandom and GoRogue, and it is safe to say they follow similar philosophies.
GoRogue's author, Chris3606, is a major contributor to this library. GoRogue is also licensed under MIT:
- [GoRogue](https://github.com/Chris3606/GoRogue)
- [GoRogue License](https://github.com/Chris3606/GoRogue/blob/master/LICENSE)


### Troschuetz.Random
ShaiRandom's optional TroschuetzCompat library depends on Troschuetz.Random to help bridge compatibility between that now-archived library and this one. With Troschuetz.Random now
archived, compatibility with it is probably less important than it used to be, but older versions of GoRogue used Troschuetz.Random, and it's still a well-made library.
Troschuetz.Random is also licensed under MIT:
- [Troschuetz.Random](https://gitlab.com/pomma89/troschuetz-random/-/tree/master)
- [Troschuetz.Random License](https://gitlab.com/pomma89/troschuetz-random/-/blob/master/LICENSE)

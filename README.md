# gear-physics
Testing out the physics of 2D gears using MonoGame

The gear-physics contains all the test code I wrote to try to figure out how everything should work. The cleaned up code is in the GearSim project.

[![Gears Gears Gears](http://img.youtube.com/vi/ocbXbJopxXM/0.jpg)](http://www.youtube.com/watch?v=ocbXbJopxXM "Video of the output")

Already implemented

- Generation of involute gears, with parameters for 
    - Number of teeth
    - Diametral pitch (size of teeth expressed as number of teeth per inch or centimeter of the pitch diameter)
    - Pressure angle
- Placement and rotation so that gears rotate perfectly interlocked

TODO:
- 3D model generation
- UV coordinate generation
- Texture generation
- Internal gears, as in [this example](https://geargenerator.com/)
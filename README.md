# Procedural Building Facade Generation in Unity

### About
This project is made for my master thesis at KTH Royal Institute of Technology. 

I will investigate procedurally generating building facades in Unity. Theoretically, much of it is based on the following paper: [Selection Expressions for Procedural Modeling](https://www.researchgate.net/publication/328484066_Selection_Expressions_for_Procedural_Modeling). I hope to achieve scientific novelty by implementing the methods in Unity, which has not yet been done, as well as performing many forms of evaluations that the original authors did not do. 

### Unity
I am using Unity version 2020.3.29f1 (LTS). 

### Functionality

## Damage & age
Buildings and their materials tend to deteriorate over time. I attempt to model this using a basic technique where the user selects a backdrop, and if they desire an object that should affect the backdrop. This is done by sampling the backdrop's texture against the locations of the desired object(s). A ray is cast at regular intervals, and if they collide with a desired object, this location is marked on a single channel texture mask, where an alpha value of 1 is "inside", e.g. a desired object was hit, and 0 is "outside", so not hit. Based on this black and white texture mask, a Signed Distance Field (SDF) is applied, which effectively causes the area to grow outwards. Then the user can choose to apply some noise, e.g. perlin, to resulting area, which causes a more natural looking area of deterioration. Finally, this is used as a texture mask to apply a desired deteriorative effect, e.g. rust, mold, salt damage, etc. See the example images below for how this works. 
<p align="left">
  <img src="/Assets/Images/ProgressPics/Damage%20and%20age/baseFacade.png" width="500" />
  <img src="/Assets/Images/ProgressPics/Damage%20and%20age/facadeMask.png" width="256" />
    <img src="/Assets/Images/ProgressPics/Damage%20and%20age/facadeMaskSDF.png" width="256" />
  <img src="/Assets/Images/ProgressPics/Damage%20and%20age/facadeMaskSDF%2BPerlinNoise.png" width="256" />
  <img src="/Assets/Images/ProgressPics/Damage%20and%20age/facadeMasked.png" width="256" />
    <img src="/Assets/Images/ProgressPics/Damage%20and%20age/facadeMaskApplied.png" width="256" />
</p>

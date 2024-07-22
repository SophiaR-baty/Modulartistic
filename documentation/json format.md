# JSON Format
To generate images or animations a ```.json``` file containing information about the image(s) or animation(s) has to be specified. This page will introduce you to the valid syntax and structure of these json files. 

There exists a schema which is used to validate the json files. It is recommended to use an IDE that automatically validates your json. One example is Visual Studio Code. At the end of this page you will see how you can configure a folder so that vscode automatically gives suggestions, validates your json and generate imgages/animations for it.   

## Underlying structure
To generate an image/animation you will need a GenerationData json. At the root of such a json there is a list of the data you want to generate. 
```json
[
	..., 
	...,
	...
]
```

The data at the moment is one of 4 possible object types: 
- StateOptions
- State
- StateSequence
- StateTimeline

Each of these types has its own json format that will be explained in the following chapters. 

## StateOptions
**StateOptions** contain global information about images and animations like Size and Framerate. The values in a StateOptions object will be applied to each image until a new StateOptions object appears in the list. 

StateOptions objects can have the following properties specified: 
- *Width* - An Integer or string containing an expression specifying the width of an image or animation
- *Height* - An Integer or string containing an expression specifying the height of an image or animation
- *Framerate* - An Integer or string containing an expression specifying the frames per second of an animation
- *FunctionRedHue* - A string containing an expression used to calculatethe Red or Hue component (depending on UseRGB) of a pixel
- *FunctionGreenSaturation* - A string containing an expression used to calculate the Green or Saturation component (depending on UseRGB) of a pixel
- *FunctionBlueValue* - A string containing an expression used to calculate the Blue or Value component (depending on UseRGB) of a pixel
- *FunctionAlpha* - A string containing an expression used to calculate the Alpha component (Transparency) of a pixel
- *InvalidColorGlobal* - A bool (true or false), if true uses a specified color if any Function returns an invalid value, if false only uses the component of the function that returned an invalid number
- *CircularMod* - a bool, if true uses "Circular Mod" instead of regular modulo on pixel values. This is useful for color components other than Hue to simulate a circular behavior.
	- "Circular modulo" is defined as $n \text{ cmod } k = 2\cdot(n \mod k)$ if that result is less than $k$ and $n \text{ cmod } k = 2\cdot(k-(n \mod k))$ otherwise
- *UseRGB* - a bool, if true uses RGB color components instead of HSV
- *AddOns* - a list of strings containing paths to AddOn files

## State
**State** objects represent a single image that is generated. It contains all the data that's needed to generate an image (that is not already specified in StateOptions). 

State objects can have the following properties specified: 
- *Name* - A string specifying the Name of the state
- *X0* - A number specifying the x-Coordinate that will be in the middle of the screen.
- *Y0* - A number specifying the y-Coordinate that will be in the middle of the screen.
- *XRotationCenter* - A number specifying the x-Coordinate around which is rotated.
- *YRotationCenter* - A number specifying the y-Coordinate around which is rotated.
- *XFactor* - A number specifying the factor by which the x-Coordinates will be scaled.
- *YFactor* - A number specifying the factor by which the y-coordinates will be scaled.
- *Rotation* - A number specifying the Amount of degrees the image will be rotated.
- *Mod* - A number specifying the Modulus Number by which all functions are taken modulo.
- *ModLowerLimit* - A number specifying the Lower Limit of the Modulus number. Values below will be treated as invalid.
- *ModUpperLimit* - A number specifying the Upper Limit of the Modulus number. Values above will be treated as invalid.
- *ColorRedHue* - A number specifying the Red or Hue Offset or Constant.
- *ColorGreenSaturation* - A number specifying the Green or Saturation Offset or Constant. Has to be from 0-1.
- *ColorBlueValue* - A number specifying the Blue or Value Offset or Constant. Has to be from 0-1.
- *ColorAlpha* - A number specifying the Alpha Offset or Constant.
- *InvalidColorRedHue* - A number specifying the Red or Hue Value for invalid results.
- *InvalidColorGreenSaturation* - A number specifying the Green or Saturation Value for invalid results. Has to be from 0-1.
- *InvalidColorBlueValue* - A number specifying the Blue or Value Value for invalid results. Has to be from 0-1.
- *InvalidColorAlpha* - A number specifying the Alpha Value for invalid results.
- *ColorFactorRedHue* - A number specifying the factor by which red or hue is scaled at the end.
- *ColorFactorGreenSaturation* - A number specifying the factor by which green or saturation is scaled at the end.
- *ColorFactorBlueValue* - A number specifying the factor by which blue or value is scaled at the end.
- *Parameters* - A list of numbers used as custom parameters in your function.

> **Notes:**
> - For all number properties you can specify an expression that uses other properties - in that case be careful of the calculation order (top to bottom) and default values for not (yet) specified properties
> - Color and InvalidColor values will use the color model defined in the StateOptions by the UseRGB property - except ColorFactors
> - ColorFactors will be applied after the actual pixel value has been calculated and will scale the opposite color components
> - If rotation was specified the image will be rotated around the rotation center, after that (x_0, y_0) is forced to the middle of the image

## StateSequence
**StateSequence** objects are one way to create animations. It contains a list of ```Scene``` objects that are simply a State, an Easing Function and a length. It then creates an animation interpolating values between states of consecutive Scenes using the easing function. 

The StateSequence objects can have the following properties specified: 

- *Name* - A string specifying the name of this sequence
- *Scenes* - A list containing "Scene" objects

The Scene objects can have the following properties specified:

- *State* - A state object that the animation will have at the start of this scene
- *Length* - A number specifying the duration in seconds it takes to progress to the next scene
- *EasingType* - A string specifying the kind of easing applied to the scene 

> **Notes:**
> - Supported Easing types are the ones defined (and explained) on https://easings.net/
> - To make the animation loop by transitioning from the last scene back to the first give the last scene a length greater than 0


## StateTimeline
A **StateTimeline** lets you create animations just like with StateSequences. However there are differences. It has a **specific Length**, a Collection of **Events** and a **Base State** that is shown when no Events are active. You can compare it to a piece of music where a Note is an event.

StateTimeline objects can have the following properties specified: 
- *Name* - A string specifying the name of this animation
- *Length* - A number specifying the Length of this animation in milliseconds
- *Base* - A state object to display when no events are active
- *Events* - A list of event objects

Event objects can have the following properties specified: 
- *StartTime* - A number specifying the start of this event in milliseconds
- *Length* - A number specifying the length of this event in milliseconds
- *AttackTime* - A number specifying the time in milliseconds this event takes to reach its peak values
- *AttackEasingType* - A string specifying the easing type used for attack
- *DecayTime* - A number specifying the time in milliseconds this event takes to decay from its attack values to sustain values
- *DecayEasingType* - A string specifying the easing type used for decay
- *ReleaseTime* - A number specifying the time in milliseconds this event takes to reach base values after its end
- *ReleaseEasingType* - A string specifying the easing type used for release
- *PeakValues* - An object containing State Properties specifying the values at its peak
- *SustainValues* - An object containing State Properties specifying the values at sustain

> **Notes:**
> - For more clarification about what is meant by Attack, Decay, Sustain and Release read about [envelope in music](https://en.wikipedia.org/wiki/Envelope_(music))
> - Creating StateTimelines by hand is rather uninteresting in my opinion. When adding this I had a different purpose in mind: creating animations as visualization of [midi files](https://en.wikipedia.org/wiki/MIDI#Standard_files). However this functionality is not fully supported yet

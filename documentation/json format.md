# JSON Format
To generate images a ```json``` file containing information about the image(s) or animation(s) has to be specified on the command line. This page will introduce you to the valid syntax and structure of these ```json``` files. 

## Underlying structure
The root element of the ```json``` will always be a list containing several objects that are linearly processed. 
```json
[
	"object1", 
	"object2", 
	"..."
]
```

These objects can be one of three types (there is a fourth one but it will get an extra page): 
- GenerationArgs
- State
- StateSequence

An object (in general for ```json``` files) consists of name/value pairs. An object might look like this: 
```json
{
	"name1": 1,
	"name2": 2
}
```


The different object types have differently named properties though, so the following chapters will guide you through what properties can be specified for each object type. 

## GenerationArgs
**GenerationArgs** contains global information about images and animations like image Size. The values in a GenerationArgs object will be applied to each image until a new GenerationArgs object appears in the list. 
GenerationArgs objects can have the following properties specified: 

| name                     | value type      | default value    | Description                                                                                                |
| ------------------------ | --------------- | ---------------- | ---------------------------------------------------------------------------------------------------------- |
| ```Size```               | ```[int,int]``` | ```[500, 500]``` | The Size in pixels that images/animations will have                                                        |
| ```Framerate```          | ```int```       | ```12```         | The Framerate in Frames per Second for animations                                                          |
| ```Circular```           | ```bool```      | ```true```      | If true, makes variable values "wrap around", such that their max value = min value                        |
| ```InvalidColorGlobal```     | ```bool```      | ```false```       | If true, only one function has to return an invalid result for all Color values to use their invalid value |
| ```UseRGB```             | ```bool```      | ```false```      | If true, uses RGB instead of HSV values. Constants and Functions of the other color type will be ignored  |
| ```HueFunction```        | ```string```    | ```"0"```                 | The function by which hue of each pixel is calculated                                                                                                           |
| ```SaturationFuntion```  | ```string```    | ```"0"```                 | The function by which saturation of each pixel is calculated                                                                                                           |
| ```ValueFunction``` | ```string```    |  ```"0"```                | The function by which value of each pixel is calculated                                                                                                           |
| ```RedFunction```        | ```string```    | ```"0"```                 | The function by which red of each pixel is calculated                                                                                                           |
| ```GreenFunction```      | ```string```    | ```"0"```                 | The function by which green of each pixel is calculated                                                                                                           |
| ```BlueFunction```       | ```string```    | ```"0"```                 | The function by which blue of each pixel is calculated                                                                                                           |
| ```AlphaFunction```      | ```string```    | ```"0"```                 | The function by which alpha (transparency) of each pixel is calculated                                                                                                           |
| ```AddOns```             | ```list```      | ```[]```                 | AddOns/Plugins as a list of ```.dll``` files                                                                                                          |

## State
**State** objects represent a single image that is generated. It contains all the data not specified in GenerationArgs that's needed to generate an image via the algorithm. 
State objects support the following Property names: 

| name                         | value type   | default value                        | Description                                                                                                   |
| ---------------------------- | ------------ | ------------------------------------ | ------------------------------------------------------------------------------------------------------------- |
| ```Name```                   | ```string``` | ```"State"```                        | The name of the state which will also be the filename of the image                                            |
| ```X0```                     | ```double``` | ```0```                              | The X Coordinate in the middle of the image                                                                   |
| ```Y0```                     | ```double``` | ```0```                              | The Y Coordinate in the middle of the image                                                                   |
| ```XRotationCenter```             | ```double``` | ```0```                              | The X Coordinate around which it rotates                                                                      |
| ```YRotationCenter```             | ```double``` | ```0```                              | The Y Coordinate around which it rotates                                                                      |
| ```XFactor```                | ```double``` | ```1```                              | All X Values will be multiplied by this                                                                       |
| ```YFactor```                | ```double``` | ```1```                              | All Y Values will be multiplied by this                                                                       |
| ```Rotation```               | ```double``` | ```0```                              | The rotation angle in degrees                                                                                 |
| ```Mod```                    | ```double``` | ```500```                            | All functions are taken modulo this number                                                                    |
| ```ModLimLow```              | ```double``` | ```0```                              | If the result of the modulo is outside of the range ModLimLow-ModLimHigh, the result will be treated as invalid |
| ```ModLimUp```               | ```double``` | ```500```                            | If ModLimHigh < ModLimLow -> everything that is INSIDE the range ModLimLow-ModLimHigh will be treated as invalid  |
| ```ColorHue```               | ```double``` | ```0```                              | The Constant Hue or offset                                                                                    |
| ```ColorSaturation```        | ```double``` | ```0```                              | The Constant Saturation or offset                                                                                                              |
| ```ColorValue```             | ```double``` | ```0```                              | The Constant Value or offset                                                                                                              |
| ```InvalidColorHue```        | ```double``` | ```0```                              | The Invalid Hue                                                                                                              |
| ```InvalidColorSaturation``` | ```double``` | ```0```                              | The Invalid Saturation                                                                                                              |
| ```InvalidColorValue```      | ```double``` | ```0```                              | The Invalid Value                                                                                                              |
| ```ColorRed```               | ```double``` | ```0```                              | The Constant Red or offset                                                                                                              |
| ```ColorGreen```             | ```double``` | ```0```                              | The Constant Green or offset                                                                                                              |
| ```ColorBlue```              | ```double``` | ```0```                              | The Constant Blue or offset                                                                                                              |
| ```InvalidColorRed```        | ```double``` | ```0```                              | The Invalid Red                                                                                                              |
| ```InvalidColorGreen```      | ```double``` | ```0```                              | The Invalid Green                                                                                                              |
| ```InvalidColorBlue```       | ```double``` | ```0```                              | The Invalid Blue                                                                                                              |
| ```ColorAlpha```             | ```double``` | ```0```                              | The Constant Alpha or offset                                                                                                              |
| ```InvalidColorAlpha```      | ```double``` | ```1```                              | The Invalid Alpha                                                                                                              |
| ```Parameters```                   | ```list```   | ```[0, 0, 0, 0, 0, 0, 0, 0, 0, 0]``` | The parameters i_0-i_9    |

> [!tip]
> When specifying functions in GenerationArgs you can use `i` instead of `i_0` and `j` instead of `i_1`
## StateSequence
A **StateSequence** object is one strategy to create animations. It contains a list of ```Sequence``` objects that are simply a State, an Easing Function and a length. It then creates an animation easing between consecutive Scenes (or rather their states) for the specified length. 
The StateSequence object therefore is relatively simple. It only has the following 2 properties:  

|       name       | value type         | default value                 | Description                                                   |
|----------------| ------------------ | ----------------------------- | ------------------------------------------------------------- |
|   `Name`    | `string` | `"StateSequence"` | The name of the StateSequence which will also be the filename of the animation |
|   `Scenes`   | `List of Scene object`        | `[]`                       | A list of `Scene` objects       |

But as you can see there is another object type used here called a `Scene`. Scenes have the following Property Names: 

|       name       | value type         | default value                 | Description                                                   |
|----------------| ------------------ | ----------------------------- | ------------------------------------------------------------- |
|   ```State```    | ```state object``` | State with all default values | The state between which is animated, can be seen as keyframes |
|   ```Length```   | ```double```       | ```3```                       | The length of animating between this and the next scene       |
| ```EasingType``` | ```string```       | ```"Linear"```                | The easing function of the animation                          |

> [!example]- Currently supported EasingTypes
> Currently supported EasingTypes are: 
> - Linear
> - SineIn
> - SineOut
> - SineInOut
> - ElasticIn
> - ElasticOut
> - ElasticInOut
> - BounceOut
>   
>   More are planned for the future. 
>   For more information about these easing types, see https://easings.net/

## StateTimeline
A **StateTimeline** lets you create animations just like with StateSequences. However there are differences. It has a **specific Length**, a Collection of **Events** and a **Base State** that is shown when no Events are active. You can compare it to a piece of music where the Notes are the events. 
A StateTimeline has the following properties: 

|       name       | value type         | default value                 | Description                                                   |
|----------------| ------------------ | ----------------------------- | ------------------------------------------------------------- |
|   `Name`    | `string` |  | The name of the StateTimeline which will also be the filename of the animation |
|   `Length`   | `int`       |                        | The length of The StateTimeline in milliseconds       |
| `Base` | `state object`       |   | The state to display when no Events are active                          |
| `Events` | `List of Event object`       | | A list of `Event` objects                          |

Like with StateSequences these objects also male use of another types this time called `Event`. Events have the following Properties: 

|       name       | value type         | default value                 | Description                                                   |
|----------------| ------------------ | ----------------------------- | ------------------------------------------------------------- |
|   `StartTime`    | `int` |  | The time in milliseconds at which this Event starts |
|   `Length`   | `int`       | | The length of the Event in milliseconds       |
| `AttackTime` | `int`       |   | The time in milliseconds for this Event to reach its peak values    |
| `AttackEasingType` | `string`       |                 | The EasingType for the Attack                       |
| `DecayTime` | `int`       |   | The time in milliseconds for this Event to decay from its peak values to its sustain values   |
| `DecayEasingType` | `string`       |                 | The EasingType for the Decay                         |
| `ReleaseTime` | `int`       |   | The time in milliseconds for this Event to reach base values after its end |
| `ReleaseEasingType` | `string`       |                 | The EasingType for the Release                          |
| `PeakValues` | `object`       |   | The  peak values as key-value-pairs of state properties |
| `SustainValues` | `object`       |  | The  sustain values as key-value-pairs of state properties |

For more clarification about what is meant by Attack, Decay, Sustain and Release read about [envelope in music](https://en.wikipedia.org/wiki/Envelope_(music)). 

Creating StateTimelines by hand is rather uninteresting in my opinion. When adding this I had a different purpose in mind: creating animations as visualization of [midi files](https://en.wikipedia.org/wiki/MIDI#Standard_files). For this purpose there is an additional page where you can read about how to do this using [StateTimelineTemplate](<state timeline template>). 

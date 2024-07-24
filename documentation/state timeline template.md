# State Timeline Template
A **StateTimelineTemplate** defines a Base State as well as a Set of default Events with the intention of creating animations from [midi files](https://en.wikipedia.org/wiki/MIDI#Standard_files). These templates are also specified in json format. 

A StateTimelineTemplate has the following structure: 
```json
{
  "Name": "<name>",
  "Metadata": "<GenerationArgs>",
  "Base": "<State>",
  "Events": [
    "<event1>",
    "<event2>",
    "..."
  ]
}
```

It consists of following properties: 
- `Name`
- `Metadata`: this is a GenerationArgs element as specified [here](<json format#JSON Format#GenerationArgs>)
- `Base`: this is a State element as specified [here](<json format#JSON Format#State>)
- `Events`: A list of `Event` elements, an Event has following Properties: 
	- `Channel`: The Channel of the MIDI this Event is used for, only use 1 channel for 1 Event
	- `AttackTime`: The time to reach the PeakValue
	- `AttackEasingType`: The Easing for the Attack
	- `DecayTime`: The time to get from PeakValue to SustainValue
	- `DecayEasingType`: The Easing for the Attack
	- `ReleaseTime`: The time to get from SustainValue to Base after the end of the note
	- `ReleaseEasingType`: The Easing for Release
	- `PeakValueMappings`: Key-Value-Pairs for Peak
	- `SustainValueMappings`: Key-Value-Pairs for Sustain

The Keys in the Mappings are essentially the same as the Properties of States excluding Name. Additionally Parameters have to be written as `i0`, `i1`, ..., `i9`. 
The Values in the Mappings are of type string because they may not only contain constants but also expressions with the following variables: 
- `Length`
- `NoteNumber`
- `Octave`
- `Velocity`
- `OffVelocity`
- `StartTime`
- `EndTime`

AddOn functions are currently not supported but it is planned. 

>[!example] 
>
>
>```json
>{
>	"Name": "example_template",
>	"Metadata": {
>		"HueFunction": "x*y",
>		"Framerate": 12,
>		"Size": [640, 360],
>		"InvalidColorGlobal": true
>	},
>	"Base": {
>		"Name": "base_state",
>		"Mod": 640,
>		"ModLimLow": 0,
>		"ModLimUp": 80,
>		"ColorSaturation": 1,
>		"InvalidColorAlpha": 1,
>		"InvalidColorHue": 0,
>		"InvalidColorSaturation": 0,
>		"InvalidColorValue": 0
>	},
>	"Events": [
>		{
>			"Channel": 0,
>			"AttackTime": 500,
>			"AttackEasingType": "Linear",
>			"DecayTime": 500,
>			"DecayEasingType": "Linear",
>			"ReleaseTime": 500,
>			"ReleaseEasingType": "Linear",
>			"PeakValueMappings": {
>				"ModLimLow": "10*Velocity",
>				"ModLimUp": "10*Velocity+80"
>			},
>			"SustainValueMappings": {
>				"ModLimLow": "7*Velocity",
>				"ModLimUp": "7*Velocity+80"
>			}
>		}
>	]
>}
>
>```
# Defining Functions
This Page shows how to write functions, which operators, functions and variables you can use. The last chapter gives some example functions to try out and experiment with. For more and full examples look at the examples page.  
## Operators
For Evaluating the functions defined in GenerationArgs the program uses the [NCalc package](https://github.com/ncalc/ncalc) therefore all functions and operators that work with it will also work with Modulartistic. 

Basic operators that are supported are: 
- \+ (Plus)
- \-  (Minus)
- \* (Times)
- / (Divided by)
- % (Modulo)

> [!note]
> NCalc also has booleans, logical and relational operators, bitwise operators. They will work, but the end result has to be a number, therefore you should only use them if you know what you're doing. 
> For more info visit https://github.com/ncalc/ncalc/wiki/Operators

## Variables
There are certain variables you can use in your functions for pixel color calculation that will change depending on the State being generated and the current pixel being colored: 
- `x` and `y` will be the current pixel coordinates being calculated
- `Th` and `r` are $\theta$ and $r$, the [polar coordinates](https://en.wikipedia.org/wiki/Polar_coordinate_system) of x and y 
- `num` is the Mod property of the State currently being generated
- `i_0`-`i_9` are the Parameters of the State being generated

>[!note]
>`Th` and `r` will be calculated as $\theta = \frac{180\cdot \arctan(x,y)}{\pi}$ and $r=\sqrt{x^2+y^2}$
>You may use `i` instead of `i_0` and `j` instead of `i_1`. 

## Functions
The following section contains a list of functions that come with NCalc: 
- `Sin`, `Cos`, `Tan`, `Asin`, `Acos`, `Atan` (trigonometric functions, take 1 argument)
- `Floor`, `Ceil`, `Round`, `Truncate` (rounding functions, take 1 argument)
- `Pow`, `Sqrt`, `Exp`, `Log`, `Log10` (power/exponential functions `Pow` and `Log` take 2 arguments, the others take 1 argument)
- `Abs`, `Sign`, `Min`, `Max` (`Min` and `Max` take 2 arguments, the others take 1 argument)

For more information about how these functions work, check the [NCalc Wiki](https://github.com/ncalc/ncalc/wiki/Functions). 

## AddOns
It's possible to define additional Functions by adding AddOns to the GenerationArgs. They are provided as an array of strings, each string being the file location of a .dll file containing the additional Functions. 

Modulartistic already comes with certain AddOns: 
- `MathFunctions.dll`
- `ImageFunctions.dll`
- `DebugFunctions.dll`

Before using them make sure, you understand the functions they define. An overview will be provided in their specific sections. 

> [!warning]
> Only use AddOns from trusted sources since the code in them can be executed on your PC. It's always best to confirm yourself that the AddOn is trustworthy by checking its Source Code. 

## Examples
This section shows some example functions. Feel free to try them out yourself and experiment with changing values: 
- `x*y`
- `x*x + y*y` or `Pow(x, 2) + Pow(y, 2)`
- `Pow(x, i) + Pow(y, i)`
- `Th - r`
- `num * Sin(x*y)`
- `num * Sin(x) - y`
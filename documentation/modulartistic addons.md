# Modulartistic AddOns
This page explains how to use AddOns as well as the functions defined in the AddOns Modulartistic comes with. 

 > [!warning] 
 > The AddOns that come with Modulartistic may change at any point, since they are not final as of now. Especially MathFunctions will be more structured in the future. 

## General Usage
AddOns are dll files containing additional functions to use for image generation. To use AddOns you have to specify the corresponding path in the AddOns Property of a GenerationArgs element. That can look like this: 
```json
{
	"AddOns": [
		"MathFunctions.dll",
		"ImageFunctions.dll"
	]
}
```

Modulartistic will treat these paths as either relative to the addon folder located in the app directory or as absolute paths. It is recommended to place AddOns in the folder. 

## MathFunctions
This AddOn defines several unrelated functions for mathematical concepts. Mostly to find out how they would look. 

In this AddOn following functions are defined: 
- `Product(x, y)` returns `x*y`
- `Circle(x, y)` returns `x*x + y*y`
- `Gamma(x)` returns the Gamma function of x: $\Gamma(x)$ 
- `GammaLn(x)` returns the logarithm of the Gamma function of x
- `Binomial(x, y)` returns the Binomial coefficient $\binom{x}{y}$ 
- `Harmonic(n)` returns the nth harmonic number
- `GeneralHarmonic(n, m)` returns the general harmonic number of order n of m
- `RandomNumber(min, max)` returns a random number between min and max
- `Mandelbrot(x, y, d)` if $x+y\cdot i$ is in the Mandelbrot Set, returns the absolute value of the complex number it approaches, otherwise returns NaN, does `d` iterations
- `Juliaset(z_x, z_y, c_x, c_y, d)` similar to Mandelbrot but with Juliaset
- ...

## ImageFunctions
This AddOn defines several functions for processing existing images. each of the functions comes with two variants: 
`function(x, y, absoulute_filepath)`
`function(x, y, absolute_folderpath, i)`
The first takes an absolute path to a file while the second takes an absolute path to a directory and an index `i` and it will select the `i`th image from that directory. (files are sorted by alphabet)

In the following I will only give the name of the function and a short explanation of what it does: 
- `ImageHue` returns the Hue at specified coordinates (from 0-1 not 0-360)
- `ImageBrightness` returns the Brightness at specified coordinates
- `ImageSaturation` returns the Saturation at specified coordinates
- `ImageValue` returns the Value at specified coordinates
- `ImageXDiff` returns the x-differentiated at specified coordinates
- `ImageYDiff` returns the y-differentiated at specified coordinates
- `ImageDirection` returns the direction at specified coordinates (from -pi-pi)
- `ImageGradient` returns the image gradient at specified coordinates

If the image wasn't found or the coordinates were out of the images range, each function returns NaN. 

## DebugFunctions
This AddOns main purpose is to help debugging. It only defines one function, which draws the x and y axis as well as a point in similar distances. 

- `Debug(x, y, th, n)` 

`th` defines the thickness of the axis and the point while `n` determines how far the points should be apart. 

## Suggestions
If you have ideas for new functions but lack the know how to make addons yourself, feel free to open an issue. 
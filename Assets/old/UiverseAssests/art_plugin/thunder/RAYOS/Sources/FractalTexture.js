// This script is placed in public domain. The author takes no responsibility for any possible harm.

var gray = true;
var width = 128;
var height = 128;

var lacunarity = 6.18;
var h = 0.69;
var octaves = 8.379;
var offset = 0.75;
var scale = 0.09;

var offsetPos = 0.0;

private var texture : Texture2D;
private var perlin : Perlin;
private var fractal : FractalNoise;

function Start ()
{
	texture = new Texture2D(width, height, TextureFormat.RGB24, false);
	GetComponent.<Renderer>().material.mainTexture = texture;
}

function Update()
{
	Calculate();
}

function Calculate()
{
	if (perlin == null)
		perlin = new Perlin();
	fractal = new FractalNoise(h, lacunarity, octaves, perlin);
	
	for (var y = 0;y<height;y++)
	{
		for (var x = 0;x<width;x++)
		{
			if (gray)
			{
				var value = fractal.HybridMultifractal(x*scale + Time.time, y * scale + Time.time, offset);
				texture.SetPixel(x, y, Color (value, value, value, value));
			}
			else
			{
				offsetPos = Time.time;
				var valuex = fractal.HybridMultifractal(x*scale + offsetPos * 0.6, y*scale + offsetPos * 0.6, offset);
				var valuey = fractal.HybridMultifractal(x*scale + 161.7 + offsetPos * 0.2, y*scale + 161.7 + offsetPos * 0.3, offset);
				var valuez = fractal.HybridMultifractal(x*scale + 591.1 + offsetPos, y*scale + 591.1 + offsetPos * 0.1, offset);
				texture.SetPixel(x, y, Color (valuex, valuey, valuez, 1));
			}
		}	
	}
	
	texture.Apply();
}
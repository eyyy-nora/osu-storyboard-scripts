using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;

/**
 * Snow particle script by eyyy
 * https://osu.ppy.sh/users/7970325
 * https://github.com/eyyy-nora/osu-storyboard-scripts
 */
namespace StorybrewScripts
{
  public class SnowParticles : StoryboardObjectGenerator
  {
    [Configurable(DisplayName = "Start Time")]
    public double startTime = 0;

    [Configurable(DisplayName = "End Time")]
    public double endTime = 0;

    [Configurable(DisplayName = "Fade Time")]
    public double fadeTime = 2000;

    [Configurable(DisplayName = "Falling Duration")]
    public double duration = 4000;

    [Configurable(DisplayName = "Falling Steps")]
    public int steps = 5;

    [Configurable(DisplayName = "Particle Density")]
    public double density = 1;

    [Configurable(DisplayName = "Images (',' separated)")]
    public string particleFiles;

    [Configurable(DisplayName = "Particle Spread")]
    public double spread = 0.5;

    [Configurable(DisplayName = "Opacity")]
    public double opacity = 1;

    [Configurable(DisplayName = "Min X")]
    public float minX = -160f;

    [Configurable(DisplayName = "Min Y")]
    public float minY = -100f;

    [Configurable(DisplayName = "Max X")]
    public float maxX = 800f;

    [Configurable(DisplayName = "Max Y")]
    public float maxY = 480f;

    [Configurable(DisplayName = "Min Particle Scale")]
    public float minScale = 0.4f;

    [Configurable(DisplayName = "Max Particle Scale")]
    public float maxScale = 0.6f;

    [Configurable(DisplayName = "Pileup Time")]
    public double pileupTime = 1000;

    const double densityBase = 80;
    const double spreadBase = 100;

    public override void Generate()
    {
      if (particleFiles == null) return;
      var files = particleFiles.Split(',');
      if (files.Length == 0) return;
      var step = Math.Max(10, densityBase / density);
      var loopCount = (int)Math.Ceiling((endTime - startTime) / duration) + 1;
      var particleCount = density * densityBase;

      for (var i = 0; i < particleCount; ++i)
      {
        var particle = GetLayer("").CreateSprite(RandomElement(files), OsbOrigin.Centre);
        particle.StartLoopGroup(startTime - (duration + pileupTime) * Random(0f, 1f), loopCount);
        var startPos = new Vector2(Random(minX, maxX), minY);
        var endPos = new Vector2(startPos.X + (float)Random(-spreadBase * spread, spreadBase * spread), maxY);
        var fromPos = startPos;
        var fromRotation = Random(-0.2d, 0.2d);
        var scale = Random(minScale, maxScale);
        particle.Move(OsbEasing.None, 0, 0, startPos, startPos);
        particle.Rotate(OsbEasing.None, 0, 0, fromRotation, fromRotation);
        particle.Scale(OsbEasing.None, 0, 0, scale, scale);
        particle.Fade(OsbEasing.None, 0, 0, 0, 1);
        var stepDuration = duration / (steps + 1);
        for (var j = 0; j < steps; ++j)
        {
          var toPos = RandomizeVector(LerpVec(startPos, endPos, (1f / steps) * j), (float)(spreadBase * spread));
          var toRotation = fromRotation + Random(-0.2d, 0.2d);
          particle.Move(OsbEasing.None, stepDuration * j, stepDuration * (j + 1), fromPos, toPos);
          particle.Rotate(OsbEasing.Out, stepDuration * j, stepDuration * (j + 1), fromRotation, toRotation);
          fromPos = toPos;
          fromRotation = toRotation;
        }
        particle.Move(OsbEasing.None, stepDuration * steps, stepDuration * (steps + 1), fromPos, RandomizeVector(endPos, (float)(spreadBase * spread * 0.2)));
        particle.Fade(OsbEasing.None, duration, duration + pileupTime, 1, 0);
        particle.EndGroup();
        particle.Fade(OsbEasing.None, startTime, startTime + fadeTime, 0, opacity);
        particle.Fade(OsbEasing.None, endTime - fadeTime, endTime, opacity, 0);
      }
    }

    public string RandomElement(string[] elements)
    {
      if (elements.Length == 1) return elements[0];
      return elements[Random(0, elements.Length - 1)];
    }

    public Vector2 LerpVec(Vector2 a, Vector2 b, float n)
    {
      var x = (1 - n) * a.X + n * b.X;
      var y = (1 - n) * a.Y + n * b.Y;
      return new Vector2(x, y);
    }

    public Vector2 RandomizeVector(Vector2 origin, float spread)
    {
      origin.X += Random(-spread, spread);
      origin.Y += Random(-spread * 0.4f, spread * 0.4f);
      return origin;
    }
  }
}

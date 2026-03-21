namespace Optify;

public static class OptifyAttributeSource
{
    public const string Filename = "OptifyAttribute.g.cs";
    private const string ClassName = "OptifyAttribute";
    public const string AttributeMetadataName = "Optify.OptifyAttribute";
    public const string SectionNamePropertyName = "SectionName";

    public static string Source =
        $$"""
          using System;

          namespace Optify;

          [AttributeUsage(AttributeTargets.Class)]
          public sealed class {{ClassName}} : Attribute
          {
              public string {{SectionNamePropertyName}} { get; init; }
          }
          """;
}

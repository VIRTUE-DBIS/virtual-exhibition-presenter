using UnityEngine;
using Valve.Newtonsoft.Json;
using System;

namespace Unibas.DBIS.VREP.Utils
{
  /// <summary>
  /// Taken from https://gist.github.com/zcyemi/e0c71c4f8ba8a92944a17f888253db0d
  /// </summary>
  public class Vec4Conv : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      if (objectType == typeof(Vector4))
      {
        return true;
      }

      return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var t = serializer.Deserialize(reader);
      var iv = JsonConvert.DeserializeObject<Vector4>(t.ToString());
      return iv;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Vector4 v = (Vector4)value;

      writer.WriteStartObject();
      writer.WritePropertyName("x");
      writer.WriteValue(v.x);
      writer.WritePropertyName("y");
      writer.WriteValue(v.y);
      writer.WritePropertyName("z");
      writer.WriteValue(v.z);
      writer.WritePropertyName("w");
      writer.WriteValue(v.w);
      writer.WriteEndObject();
    }
  }

  /// <summary>
  /// Taken from https://gist.github.com/zcyemi/e0c71c4f8ba8a92944a17f888253db0d
  /// </summary>
  public class Vec3Conv : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      if (objectType == typeof(Vector3))
      {
        return true;
      }

      return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var t = serializer.Deserialize(reader);
      var iv = JsonConvert.DeserializeObject<Vector3>(t.ToString());
      return iv;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Vector3 v = (Vector3)value;

      writer.WriteStartObject();
      writer.WritePropertyName("x");
      writer.WriteValue(v.x);
      writer.WritePropertyName("y");
      writer.WriteValue(v.y);
      writer.WritePropertyName("z");
      writer.WriteValue(v.z);
      writer.WriteEndObject();
    }
  }

  /// <summary>
  /// Taken from https://gist.github.com/zcyemi/e0c71c4f8ba8a92944a17f888253db0d
  /// </summary>
  public class Vec2Conv : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      if (objectType == typeof(Vector2))
      {
        return true;
      }

      return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var t = serializer.Deserialize(reader);
      var iv = JsonConvert.DeserializeObject<Vector2>(t.ToString());
      return iv;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Vector2 v = (Vector2)value;

      writer.WriteStartObject();
      writer.WritePropertyName("x");
      writer.WriteValue(v.x);
      writer.WritePropertyName("y");
      writer.WriteValue(v.y);
      writer.WriteEndObject();
    }
  }
}
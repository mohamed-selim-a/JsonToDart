﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Selim.Json
{
    public class JsonArray : JsonValue
    {
        List<JsonValue> values = new List<JsonValue>();

        public JsonArray()
        {}

        public JsonArray(params JsonValue[] values)
        {
            this.values.AddRange(values);
        }

        public JsonArray add(params JsonValue[] values)
        {
            this.values.AddRange(values);
			return this;
        }

		public JsonArray add(JsonValue value)
        {
            this.values.Add(value);
			return this;
        }

		public JsonArray add(double value)
        {
            this.values.Add(new JsonNumber(value));
			return this;
        }

		public JsonArray add(string value)
        {
            if(value!=null)
            {
                this.values.Add(new JsonString(value));
            }
            else
            {
                this.values.Add(new JsonNull());
            }
			return this;
        }

		public JsonArray add(bool value)
        {
            this.values.Add(new JsonBoolean( value));
			return this;
        }

		public JsonArray addAll(double[] arr)
		{
			foreach (double item in arr)
			{
				add(item);
			}
			return this;
		}

		public JsonArray addAll(string[] arr)
		{
			foreach (string item in arr)
			{
				add(item);
			}
			return this;
		}

		public JsonArray addAll(bool[] arr)
		{
			foreach (bool item in arr)
			{
				add(item);
			}
			return this;
		}

        public int Length { get { return values.Count; } }

       
        public JsonValue[] getValues()
        {
            return values.ToArray();
        }

        public JsonValue getValue(int index)
        {
            return values.ElementAt(index);
        }

		public string getString(int index)
		{
			JsonString tmp = (JsonString)getValue(index);
			return tmp.value;
		}

		public double getDouble(int index)
		{
			JsonNumber tmp = (JsonNumber)getValue(index);
			return tmp.value;
		}

		public int getInt(int index)
		{
			JsonNumber tmp = (JsonNumber)getValue(index);
			return (int)tmp.value;
		}

		public bool getBool(int index)
		{
			JsonBoolean tmp = (JsonBoolean)getValue(index);
			return tmp.value;
		}

		public JsonObject getObject(int index)
		{
			return (JsonObject)getValue(index);
		}

		public JsonArray getArray(int index)
		{
			return (JsonArray)getValue(index);
		}

		/// <summary>
		/// gets a double[] of all elements, all elements must be of type JsonNumber
		/// </summary>
		public double[] getAllDoubles()
		{
			double[] tmp= new double[this.Length];
			for (int i = 0; i < Length; i++)
			{
				tmp[i] = getDouble(i);
			}
			return tmp;
		}

		/// <summary>
		/// gets an int[] of all elements, all elements must be of type JsonNumber
		/// </summary>
		public int[] getAllInts()
		{
			int[] tmp = new int[this.Length];
			for (int i = 0; i < Length; i++)
			{
				tmp[i] = getInt(i);
			}
			return tmp;
		}

		/// <summary>
		/// gets a bool[] of all elements, all elements must be of type JsonBoolean
		/// </summary>
		public bool[] getAllBools()
		{
			bool[] tmp = new bool[this.Length];
			for (int i = 0; i < Length; i++)
			{
				tmp[i] = getBool(i);
			}
			return tmp;
		}

		/// <summary>
		/// gets a string[] of all elements, all elements must be of type JsonString
		/// </summary>
		public string[] getAllStrings()
		{
			string[] tmp = new string[this.Length];
			for (int i = 0; i < Length; i++)
			{
				tmp[i] = getString(i);
			}
			return tmp;
		}


		public override void toString(StringBuilder sb, int? indents)
		{
            
            if (indents != null)
            {
                //appendMany(sb, indentation, (int)indents); 
                sb.Append("["); sb.Append("\r\n");
                for (int i = 0; i < this.values.Count; i++)
                {
                    appendMany(sb, indentation, (int)indents + 1); 
                    values[i].toString(sb, indents+1);
                    if (i < values.Count - 1)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }
                appendMany(sb, indentation, (int)indents);
                sb.Append("]");
            }
            else
            {
                sb.Append("[");
                foreach (JsonValue item in this.values)
                {
                    item.toString(sb); sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);//remove the last comma
                sb.Append("]");
            }
           
		}

		public override string dartTypeName => $"List<{((Length == 0 || values[0] is JsonNull)? "Object?" : $"{values[0].dartTypeName}?")}>";

		public override string toDartMapAssignmentExpr(string name)
        {
			var notObject = (Length == 0 || !(values[0] is JsonObject));

			return $"\t\tif (this.{name} != null) {{\r\n" +
				$"\t\t\tdata['{name}'] = this.{name}!.map((v) => v{(notObject? "": "?.toMap()")}).toList();\r\n" +
				$"\t\t}}";
        }

        public override string toDartMapFetchingExpr(string name)
        {
			var notObject = (Length == 0 || !(values[0] is JsonObject));

			return $"\t\tif (map['{name}'] != null) {{\r\n" +
				$"\t\t\t{name} = [];\r\n" +
				$"\t\t\tmap['{name}'].forEach((v) {{\r\n"+
				$"\t\t\t\t{name}!.add({(notObject? "v" : $"({values[0].dartTypeName}()..fromMap(v))")});\r\n" +
				$"\t\t\t}});\r\n" +
				$"\t\t}}";
		}
    }
}

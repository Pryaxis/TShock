/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using Rests;

namespace TshockRestTestPlugin
{
	[DisplayName("JSON Status")]
	[Description("Checks to see the that the JSON response has the specified status response")]
	public class JsonValidateStatus : JsonValidate
	{
		public override void Validate(object sender, ValidationEventArgs e)
		{
			if (null != ValidateJson(sender, e))
				e.IsValid = true;
		}
	}

	[DisplayName("JSON Regexp Property")]
	[Description("Checks to see the that the JSON response contains the specified property and is matches the specified regexp")]
	public class JsonValidateRegexpProperty : JsonValidateProperty
	{
		// The name of the desired JSON property
		[DisplayName("Regexp")]
		[DefaultValue(true)]
		public new bool UseRegularExpression { get { return base.UseRegularExpression; } set { base.UseRegularExpression = value; } }
	}

	[DisplayName("JSON Error")]
	[Description("Checks to see the that the JSON response contains the specified error")]
	public class JsonValidateError : JsonValidateProperty
	{
		// The status of the JSON request
		[DisplayName("JSON Status")]
		[DefaultValue("400")]
		public new string JSonStatus { get { return base.JSonStatus; } set { base.JSonStatus = value; } }

		// The name of the desired JSON property
		[DisplayName("Property")]
		[DefaultValue("error")]
		public new string PropertyName { get { return base.PropertyName; } set { base.PropertyName = value; } }
	}

	[DisplayName("JSON Missing Parameter")]
	[Description("Checks to see the that the JSON response indicates a missing or invalid parameter")]
	public class JsonValidateMissingParameter : JsonValidateError
	{
		// The value of the desired JSON property
		[DisplayName("Missing Value")]
		public new string PropertyValue { get { return base.PropertyValue; } set { base.PropertyValue = String.Format("Missing or empty {0} parameter", value); } }
	}

	[DisplayName("JSON Invalid Parameter")]
	[Description("Checks to see the that the JSON response indicates a missing or invalid parameter")]
	public class JsonValidateInvalidParameter : JsonValidateError
	{
		// The value of the desired JSON property
		[DisplayName("Invalid Value")]
		public new string PropertyValue { get { return base.PropertyValue; } set { base.PropertyValue = String.Format("Missing or invalid {0} parameter", value); } }
	}

	[DisplayName("JSON Response")]
	[Description("Checks to see the that the JSON response contains the specified message")]
	public class JsonValidateResponse : JsonValidateProperty
	{
		// The name of the desired JSON property
		[DisplayName("Response")]
		[DefaultValue("response")]
		public new string PropertyName { get { return base.PropertyName; } set { base.PropertyName = value; } }
	}

	[DisplayName("JSON Property")]
	[Description("Checks to see the that the JSON response contains the specified property and is set to the specified value")]
	public class JsonValidateProperty : JsonValidate
	{
		// The name of the desired JSON property
		[DisplayName("Property")]
		public string PropertyName { get; set; }

		// The value of the desired JSON property
		[DisplayName("Value")]
		public string PropertyValue { get; set; }

		// Is the value a regexp of the desired JSON property
		[DisplayName("Regexp")]
		[DefaultValue(false)]
		public bool UseRegularExpression { get; set; }

		public override void Validate(object sender, ValidationEventArgs e)
		{
			RestObject response = ValidateJson(sender, e);
			if (null == response)
				return;

			if (null == response[PropertyName])
			{
				e.Message = String.Format("{0} Not Found", PropertyName);
				e.IsValid = false;
				return;
			}

			if (UseRegularExpression)
			{
				var re = new Regex(PropertyValue);
				if (!re.IsMatch((string)response[PropertyName]))
				{
					e.Message = String.Format("{0} => '{1}' !~ '{2}'", PropertyName, response[PropertyName], PropertyValue);
					e.IsValid = false;
					return;
				}
			}
			else
			{
				if (PropertyValue != (string)response[PropertyName])
				{
					e.Message = String.Format("{0} => '{1}' != '{2}'", PropertyName, response[PropertyName], PropertyValue);
					e.IsValid = false;
					return;
				}
			}

			e.IsValid = true;
			//e.WebTest.Context.Add(ContextParameterName, propertyValue);
		}
	}

	[DisplayName("JSON Has Properties")]
	[Description("Checks to see the that the JSON response contains the specified properties (comma seperated)")]
	public class JsonHasProperties : JsonValidate
	{
		// The name of the desired JSON properties to check
		[DisplayName("Properties")]
		[Description("A comma seperated list of property names to check exist")]
		public string PropertyNames { get; set; }

		//---------------------------------------------------------------------
		public override void Validate(object sender, ValidationEventArgs e)
		{
			RestObject response = ValidateJson(sender, e);
			if (null == response)
				return;
			foreach (var p in PropertyNames.Split(','))
			{
				if (null == response[p])
				{
					e.Message = String.Format("'{0}' Not Found", p);
					e.IsValid = false;
					return;
				}
			}
			e.IsValid = true;

			//e.WebTest.Context.Add(ContextParameterName, propertyValue);
		}
	}

	public abstract class JsonValidate : ValidationRule
	{
		// The status of the JSON request
		[DisplayName("JSON Status")]
		[DefaultValue("200")]
		public string JSonStatus { get; set; }

		public RestObject ValidateJson(object sender, ValidationEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.Response.BodyString))
			{
				e.IsValid = false;
				e.Message = String.Format("Empty or null response {0}", e.Response.StatusCode);
				return null;
			}
			JavaScriptSerializer serialiser = new JavaScriptSerializer();
			//dynamic data = serialiser.Deserialize<dynamic>(e.Response.BodyString);
			RestObject response = serialiser.Deserialize<RestObject>(e.Response.BodyString);

			if (JSonStatus != response.Status)
			{
				e.IsValid = false;
				e.Message = String.Format("Response Status '{0}' not '{1}'", response.Status, JSonStatus);
				return null;
			}

			return response;
		}
	}
}
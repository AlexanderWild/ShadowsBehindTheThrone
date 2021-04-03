using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class ModData {
		public string title;
		public string description;

		public List<string> tags;
		public string previewImage;

		public bool validate()
		{
			if (String.IsNullOrWhiteSpace(title))
				return false;
			if (String.IsNullOrWhiteSpace(description))
				return false;

			if (tags.Count == 0)
				tags.Add("event");

			// FIXME: need default event preview image.
			// - it probably has a special size.
			if (!File.Exists(previewImage))
				previewImage = Directory.GetCurrentDirectory() + "/data/graphics/boxImages/default.png";

			return true;
		}
	}
}

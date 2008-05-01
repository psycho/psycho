//------10--------20--------30--------40--------50--------60--------70--------80
//
// Topic.cs
// 
// Copyright (C) 2008 Piotr Zurek p.zurek@gmail.com
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Psycho.Core;

namespace Psycho.Core
{
	public class Topic : ITopic
	{
		private string text;
		private string id;
		private Topic parent;
		private Note note;
		private bool isExpanded;
		private TopicList subtopicList;
		private int totalCount;
		private string path;
		private string number;
		private int level;
		
		public Topic()
		{
			this.Text = "Topic ";
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}

		public string ID {
			get {
				if (id == null) {
					Guid guid = System.Guid.NewGuid();
					id = guid.ToString();
				}
				return id;
			}
			set { id = value; }
		}

		public Topic Parent {
			get { return parent; }
			set { parent = value; }
		}

		public Note Note {
			get { return note; }
			set { note = value; }
		}

		public bool IsExpanded {
			get { return isExpanded; }
			set { isExpanded = value; }
		}

		public bool HasNote {
			get {
				return (note != null &&
				        !string.IsNullOrEmpty(note.Text));
			}
		}

		public int TotalCount {
			get { return totalCount; }
		}

		public string Path {
			get { return path; }
		}

		public string Number {
			get { return number; }
		}

		public int Level {
			get { return level; }
		}

		public TopicList SubtopicList {
			get {
				return subtopicList;
			}
		}

		public void AddSubtopic ()
		{
			
		}

		public void InsertSubtopic (int at_index, Topic my_topic)
		{
			this.SubtopicList.Insert (at_index, my_topic);		}

		public void Delete ()
		{
		}

		public void ForEach (System.Action<Topic> action)
		{
		}
	}
}

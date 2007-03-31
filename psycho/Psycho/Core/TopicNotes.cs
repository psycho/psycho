// Copyright (C) 2006 by:
//
// Author:
//   Piotr Zurek, p.zurek@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Psycho
{
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using Psycho;

        public class TopicNotes
        {
                string text;
                Topic topic;

                public TopicNotes (Topic iTopic)
                {
                        this.topic = iTopic;
                        this.text = "";
                        //string previousText, nextText, firstAncestorText;
                        //if (this.Topic.Previous != null) previousText = this.Topic.Previous.Text;
                        //else previousText = "";
                        //if (this.Topic.Next != null) nextText = this.Topic.Next.Text;
                        //else nextText = "";
                        //if (this.Topic.FirstAncestor != null) firstAncestorText = this.Topic.FirstAncestor.Text;
                        //else firstAncestorText = "";
                        //this.Text = ("Previous: " + previousText + " /n " +
                        //                   "Next: " + nextText + " /n " +
                        //                   "First Ancestor: " + firstAncestorText);
                }

                public string Text
                {
                        get { return text; }
                        set { text = value; }
                }

                public Topic Topic
                {
                        get { return topic; }
                }

                public string GUID
                {
                        get { return this.topic.GUID; }
                }
        }
}
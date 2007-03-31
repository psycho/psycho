// Copyright (C) 2006 by:
//
// Author:
//   Piotr Zurek
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

namespace Psycho {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using Psycho;

    /// <summary>
    /// Implementation of the IModel interface for the Topic object.
    /// </summary>
    public partial class MindModel : IModel {

        private Topic centralTopic = new Topic (1234);
        private Topic currentTopic;
        public XmlDocument content;

        #region IModel Members
        private ArrayList observerList = new ArrayList ();
        private Topics newTopics = new Topics ();
        private Topics deletedTopics = new Topics ();
        private string deletedTopicPath = ("");
        private Topics changedTopics = new Topics ();
        private bool editPending;

        public Topic CurrentTopic
        {
            get { return currentTopic; }
            set { currentTopic = value; }
        }

        public Topics NewTopics
        {
            get { return newTopics; }
        }

        public Topics ChangedTopics
        {
            get { return changedTopics; }
        }

        public string DeletedTopicPath
        {
            get { return deletedTopicPath; }
        }

        public Topics DeletedTopics
        {
            get { return deletedTopics; }
        }


        public Topic CentralTopic
        {
            get { return centralTopic; }
            set { centralTopic = value; }
        }

        public int CurrentLevel
        {
            get { return currentTopic.Level; }
        }

        public bool EditPending
        {
            get { Console.WriteLine ("Edit pending {0}", editPending.ToString ()); return editPending; }
            set { editPending = value; Console.WriteLine ("Edit pending {0}", editPending.ToString ()); }
        }

        public void CreateTopic ()
        {
            if (CurrentTopic.Parent != null) {
                int currentIndex = CurrentTopic.Parent.Subtopics.IndexOf (CurrentTopic);
                Topic newTopic = new Topic (centralTopic.TotalCount);
                newTopic.Parent = CurrentTopic.Parent;
                CurrentTopic.Parent.AddSubtopicAt ((currentIndex + 1), newTopic);
                CurrentTopic = newTopic;
                newTopics.Add (newTopic);
                NotifyObservers ();
            }
        }

        public void CreateSubtopic ()
        {
            Topic newTopic = new Topic (centralTopic.TotalCount);
            newTopic.Parent = CurrentTopic;
            CurrentTopic.AddSubtopic (newTopic);
            CurrentTopic = newTopic;
            newTopics.Add (newTopic);
            NotifyObservers ();
        }

        public void DeleteTopic ()
        {
            int newIndex;
            int currentIndex;

            if (CurrentTopic.Parent != null) {
                currentIndex = CurrentTopic.Parent.Subtopics.IndexOf (CurrentTopic);
            }
            else
                return;

            Topic tempParent = this.CurrentTopic.Parent;
            Topic deletedTopic = (CurrentTopic);

            deletedTopicPath = (deletedTopic.Path);
            deletedTopics.Add (deletedTopic);

            if (CurrentTopic.Parent.Subtopics.Count == 1) {
                CurrentTopic.Parent.Subtopics.Clear ();
                CurrentTopic = tempParent;
            }
            else {
                if (currentIndex == (CurrentTopic.Parent.Subtopics.Count - 1)) {
                    newIndex = currentIndex - 1;
                }
                else {
                    newIndex = currentIndex;
                }
                CurrentTopic.Parent.Subtopics.RemoveAt (currentIndex);
                CurrentTopic = tempParent.Subtopics[newIndex];
            }
            NotifyObservers ();
        }

        public void SetTitle (string paramTitle)
        {
            CurrentTopic.Title = (paramTitle);
            changedTopics.Add (CurrentTopic);
            NotifyObservers ();
        }

        public void AddObserver (IView paramView)
        {
            observerList.Add (paramView);
            Console.WriteLine ("View: " + paramView.ToString () + " added to observer list");
        }

        public void RemoveObserver (IView paramView)
        {
            observerList.Remove (paramView);
        }

        public void NotifyObservers ()
        {
            foreach (IView view in observerList) {
                view.Update (this);
            }
            ClearChanges ();
        }

        private void ClearChanges ()
        {
            newTopics.Clear ();
            deletedTopicPath = ("");
            deletedTopics.Clear ();
            changedTopics.Clear ();
        }

        public void SetCurrent (string paramGuid, Topic paramTopic)
        {
            Topic saughtTopic = FindByGUID (paramGuid);
            CurrentTopic = saughtTopic;
            NotifyObservers ();
        }

        public void ExpandTopic (string paramGuid, bool isExpanded)
        {
            Topic ExpandedTopic = FindByGUID (paramGuid);
            ExpandedTopic.IsExpanded = (isExpanded);
        }
        #endregion
    }
}
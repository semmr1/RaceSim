using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Track
    {
        public string Name { get; set; }
        public LinkedList<Section> Sections { get; set; }

        public Track(string name, SectionTypes[] sections)
        {
            Name = name;
            Sections = ConvertToLinkedList(sections);
        }

        private LinkedList<Section> ConvertToLinkedList(SectionTypes[] sections)
        {
            LinkedList<Section> sectionList = new();
            foreach (var section in sections)
            {
                sectionList.AddLast(new Section(section));
            }
            return sectionList;
        }

        public Section NextSection(Section section)
        {
            var currentSection = Sections.Find(section);
            LinkedListNode<Section> nextSection = currentSection.Next;
            if (nextSection == null)
            {
                nextSection = Sections.First;
                return nextSection.Value;
            }
            return nextSection.Value;
        }

        public Section PreviousSection(Section section)
        {
            var currentSection = Sections.Find(section);
            LinkedListNode<Section> nextSection = currentSection.Previous;
            if (nextSection == null)
            {
                nextSection = Sections.Last;
                return nextSection.Value;
            }
            return nextSection.Value;
        }
    }
}

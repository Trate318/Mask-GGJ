using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// all time units are in miliseconds unless specified.

public class AudioClip
{
    public String audioPath;

    public int GetLength() { return 0; }


}

public class Redactions
{
    // Resource AudioClip;
    public List<Section> Sections;

    public Redactions()
    {
        Sections = new List<Section>();
    }

    //TODO: merge any overlapping sections on add
    public void AddSection(Section section)
    {
        Sections.Add(section);
    }

    public void RemoveSection(Section section)
    {
        for (int i = 0; i < Sections.Count; i++)
        {
            Section s = Sections[i];
            if (s.Equals(section))
            {
                Sections.Remove(section);
            }
        }
    }

    public Redactions Subtract(Redactions redactions)
    {
        Redactions result = new Redactions();

        foreach (Section thisSection in Sections)
        {
            foreach (Section section in redactions.Sections)
            {
                List<Section> subtractedSections = thisSection.Subtract(section);
                foreach (Section subtractedSection in subtractedSections)
                {
                    result.AddSection(subtractedSection);
                }
            }
        }

        return result;
    }
}

public struct Section
{
    // start and length should be positive only

    public int Start;
    public int Length;
    public int End => Start + Length;

    public Section(int start, int length)
    {
        this.Start = start;
        this.Length = length;
    }

    public static Section SectionByEnd(int start, int end)
    {
        return new Section(start, end - start);
    }

    public bool IsSectionOverlapping(Section section) => Start <= section.End && section.Start <= End;

    public Section Add(Section section)
    {
        if (!IsSectionOverlapping(section)) //if not overlapping return this section
        {
            return this;
        }

        return SectionByEnd(Mathf.Min(Start, section.Start), Mathf.Max(End, section.End));
    }

    public List<Section> Subtract(Section section)
    {
        if (!IsSectionOverlapping(section)) //if not overlapping return this section
        {
            return new List<Section>() { this };
        }

        int beforeLength = section.Start - Start;
        int afterLength = section.Length - Length + beforeLength;

        if (beforeLength > 0 && afterLength >= 0)
        {
            return new List<Section>() { new Section(section.Start, Start) };
        }
        else if (beforeLength <= 0 && afterLength < 0)
        {
            return new List<Section>() { new Section(End, section.End - End) };
        }
        else if (beforeLength > 0 && afterLength < 0)
        {
            return new List<Section>() { new Section(section.Start, Start), new Section(End, section.End - End) };
        }
        else
        {
            return new List<Section>() { };
        }
    }

    public override string ToString()
    {
        return $"({Start} -> {End} : {Length})";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Section)) { return false; }

        Section section = (Section)obj;

        return Start == section.Start && Length == section.Length;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// all time units are in miliseconds unless specified.

public class Redactions
{
    // Resource AudioClip;
    List<Section> sections;

    public Redactions() {}

    //TODO: merge any overlapping sections on add
    public void AddSection(Section section)
    {
        sections.Append(section);
    }

    public Redactions Subtract(Redactions redactions) {
        Redactions result = new Redactions();

        foreach (Section thisSection in sections)
        {
            foreach (Section section in redactions.sections)
            {
                List<Section> subtractedSections = thisSection.Subtract(section);
                foreach (Section subtractedSection in subtractedSections) {
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

    int Start;
    int Length;
    int End => Start + Length;

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
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// all time units are in miliseconds unless specified.

public class Mission
{
    public String PhoneCallSource;
    public String AudioClipSource;
    public Redactions CurrentRedactions;
    public Redactions GoalRedactions;

    public int Length => 1000;

    public Mission()
    {

    }
}

public class Redactions
{
    // Resource AudioClip;
    public List<Section> Sections;

    public Redactions()
    {
        Sections = new List<Section>();
    }

    public void AddSection(Section section)
    {
        // attempt combine any overlapping sections recursively
        for (int i = 0; i < Sections.Count; i++)
        {
            Section currentSection = Sections[i];

            if (currentSection.IsSectionOverlapping(section))
            {
                RemoveSection(currentSection);
                AddSection(section.Add(currentSection));
                return;
            }
        }

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

    public int ORLengthDifference(Redactions redactions)
    {
        int total = 0;

        foreach (Section sectionA in Sections)
        {
            total += sectionA.Length;

            foreach (Section sectionB in redactions.Sections)
            {
                total -= sectionA.ANDLengthDifference(sectionB);
            }
        }

        foreach (Section sectionB in redactions.Sections)
        {
            total += sectionB.Length;

            foreach (Section sectionA in Sections)
            {
                total -= sectionB.ANDLengthDifference(sectionA);
            }
        }

        return total;
    }

    public Redactions OR(Redactions redactions, Redactions workingRedactions = null)
    {
        if (workingRedactions == null)
        {
            workingRedactions = new Redactions();

            foreach (Section section in redactions.Sections)
            {
                workingRedactions.AddSection(new Section(section.Start, section.Length));
            }
        }

        foreach (Section sectionA in Sections)
        {
            foreach (Section sectionB in workingRedactions.Sections)
            {
                if (sectionA.OR(sectionB, out List<Section> result))
                {
                    workingRedactions.RemoveSection(sectionA);
                    workingRedactions.RemoveSection(sectionB);
                    foreach (Section r in result)
                    {
                        workingRedactions.AddSection(r);
                    }

                    OR(redactions, workingRedactions);
                }
            }
        }

        return workingRedactions;
    }
}

public class Section
{
    // start and length should be positive only

    public int Start;
    public int Length;
    public int End
    {
        get => Start + Length;
        set => Length = Math.Max(0, value - Start);
    }

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

    public void Clamp(int maxSize)
    {
        int newStart = Math.Max(0, Start);
        Length += Start - newStart;
        Start = newStart;
        End = Math.Min(maxSize, End);
    }

    public int ANDLengthDifference(Section section)
    {
        if (!IsSectionOverlapping(section)) { return 0; }
        return Math.Min(End, section.End) - Math.Max(Start, section.Start);
    }

    public int ORLengthDifference(Section section)
    {
        if (!IsSectionOverlapping(section)) { return Length + section.Length; }
        return Math.Abs(Start - section.Start) + Math.Abs(End - section.End);
    }

    public bool OR(Section section, out List<Section> result)
    {
        result = new List<Section>();

        if (!IsSectionOverlapping(section)) //if not overlapping return this section
        {
            return false;
        }

        Section beforeSection = new Section(Math.Min(Start, section.Start), Math.Abs(Start - section.Start));
        Section afterSection = Section.SectionByEnd(Math.Max(End, section.End), Math.Abs(End - section.End));

        if (beforeSection.Length > 0)
        {
            result.Add(beforeSection);
        }
        if (afterSection.Length > 0)
        {
            result.Add(afterSection);
        }

        return true;
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
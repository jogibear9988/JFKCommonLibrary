using System;
using System.Collections;
using System.Collections.Generic;

namespace JFKCommonLibrary.Diff
{
    public enum DiffEngineLevel
    {
        FastImperfect,

        Medium,

        SlowPerfect
    }

    public class DiffEngine
    {
        private IDiffList _source;

        private IDiffList _dest;

        private ArrayList _matchList;

        private DiffEngineLevel _level;

        private DiffStateList _stateList;

        public DiffEngine()
        {
            this._source = null;
            this._dest = null;
            this._matchList = null;
            this._stateList = null;
            this._level = DiffEngineLevel.FastImperfect;
        }

        private int GetSourceMatchLength(int destIndex, int sourceIndex, int maxLength)
        {
            int matchCount;
            for (matchCount = 0; matchCount < maxLength; matchCount++)
            {
                if (this._dest.GetByIndex(destIndex + matchCount).CompareTo(this._source.GetByIndex(sourceIndex + matchCount)) != 0)
                {
                    break;
                }
            }
            return matchCount;
        }

        private void GetLongestSourceMatch(DiffState curItem, int destIndex, int destEnd, int sourceStart, int sourceEnd)
        {

            int maxDestLength = (destEnd - destIndex) + 1;
            int curLength = 0;
            int curBestLength = 0;
            int curBestIndex = -1;
            int maxLength = 0;
            for (int sourceIndex = sourceStart; sourceIndex <= sourceEnd; sourceIndex++)
            {
                maxLength = Math.Min(maxDestLength, (sourceEnd - sourceIndex) + 1);
                if (maxLength <= curBestLength)
                {
                    //No chance to find a longer one any more
                    break;
                }
                curLength = this.GetSourceMatchLength(destIndex, sourceIndex, maxLength);
                if (curLength > curBestLength)
                {
                    //This is the best match so far
                    curBestIndex = sourceIndex;
                    curBestLength = curLength;
                }
                //jump over the match
                sourceIndex += curBestLength;
            }
            //DiffState cur = _stateList.GetByIndex(destIndex);
            if (curBestIndex == -1)
            {
                curItem.SetNoMatch();
            }
            else
            {
                curItem.SetMatch(curBestIndex, curBestLength);
            }

        }

        private void ProcessRange(int destStart, int destEnd, int sourceStart, int sourceEnd)
        {
            int curBestIndex = -1;
            int curBestLength = -1;
            int maxPossibleDestLength = 0;
            DiffState curItem = null;
            DiffState bestItem = null;
            for (int destIndex = destStart; destIndex <= destEnd; destIndex++)
            {
                maxPossibleDestLength = (destEnd - destIndex) + 1;
                if (maxPossibleDestLength <= curBestLength)
                {
                    //we won't find a longer one even if we looked
                    break;
                }
                curItem = this._stateList.GetByIndex(destIndex);

                if (!curItem.HasValidLength(sourceStart, sourceEnd, maxPossibleDestLength))
                {
                    //recalc new best length since it isn't valid or has never been done.
                    this.GetLongestSourceMatch(curItem, destIndex, destEnd, sourceStart, sourceEnd);
                }
                if (curItem.Status == DiffStatus.Matched)
                {
                    switch (this._level)
                    {
                        case DiffEngineLevel.FastImperfect:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            //Jump over the match 
                            destIndex += curItem.Length - 1;
                            break;
                        case DiffEngineLevel.Medium:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                                //Jump over the match 
                                destIndex += curItem.Length - 1;
                            }
                            break;
                        default:
                            if (curItem.Length > curBestLength)
                            {
                                //this is longest match so far
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            break;
                    }
                }
            }
            if (curBestIndex < 0)
            {
                //we are done - there are no matches in this span
            }
            else
            {

                int sourceIndex = bestItem.StartIndex;
                this._matchList.Add(DiffResultSpan.CreateNoChange(curBestIndex, sourceIndex, curBestLength));
                if (destStart < curBestIndex)
                {
                    //Still have more lower destination data
                    if (sourceStart < sourceIndex)
                    {
                        //Still have more lower source data
                        // Recursive call to process lower indexes
                        this.ProcessRange(destStart, curBestIndex - 1, sourceStart, sourceIndex - 1);
                    }
                }
                int upperDestStart = curBestIndex + curBestLength;
                int upperSourceStart = sourceIndex + curBestLength;
                if (destEnd >= upperDestStart)
                {
                    //we still have more upper dest data
                    if (sourceEnd > upperSourceStart - 1)
                    {
                        //set still have more upper source data
                        // Recursive call to process upper indexes
                        this.ProcessRange(upperDestStart, destEnd, upperSourceStart, sourceEnd);
                    }
                }
            }
        }

        public static List<DiffResultSpan> CompareStrings(string source, string destination, DiffEngineLevel level)
        {
            var inst = new DiffEngine();
            inst._level = level;
            var src = new DiffList_String(source);
            var dst = new DiffList_String(destination);
            inst.ProcessDiff(src, dst);
            return inst.DiffReport();
        }

        public static List<DiffResultSpan> CompareTextFiles(string source, string destination, DiffEngineLevel level)
        {
            var inst = new DiffEngine();
            inst._level = level;
            var src = new DiffList_TextFile(source);
            var dst = new DiffList_TextFile(destination);
            inst.ProcessDiff(src, dst);
            return inst.DiffReport();
        }

        public double ProcessDiff(IDiffList source, IDiffList destination, DiffEngineLevel level)
        {
            this._level = level;
            return this.ProcessDiff(source, destination);
        }

        public double ProcessDiff(IDiffList source, IDiffList destination)
        {
            DateTime dt = DateTime.Now;
            this._source = source;
            this._dest = destination;
            this._matchList = new ArrayList();

            int dcount = this._dest.Count();
            int scount = this._source.Count();


            if ((dcount > 0) && (scount > 0))
            {
                this._stateList = new DiffStateList(dcount);
                this.ProcessRange(0, dcount - 1, 0, scount - 1);
            }

            TimeSpan ts = DateTime.Now - dt;
            return ts.TotalSeconds;
        }


        private bool AddChanges(List<DiffResultSpan> report, int curDest, int nextDest, int curSource, int nextSource)
        {
            bool retval = false;
            int diffDest = nextDest - curDest;
            int diffSource = nextSource - curSource;
            int minDiff = 0;
            if (diffDest > 0)
            {
                if (diffSource > 0)
                {
                    minDiff = Math.Min(diffDest, diffSource);
                    report.Add(DiffResultSpan.CreateReplace(curDest, curSource, minDiff));
                    if (diffDest > diffSource)
                    {
                        curDest += minDiff;
                        report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest - diffSource));
                    }
                    else
                    {
                        if (diffSource > diffDest)
                        {
                            curSource += minDiff;
                            report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource - diffDest));
                        }
                    }
                }
                else
                {
                    report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest));
                }
                retval = true;
            }
            else
            {
                if (diffSource > 0)
                {
                    report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource));
                    retval = true;
                }
            }
            return retval;
        }

        public List<DiffResultSpan> DiffReport()
        {
            List<DiffResultSpan> retval = new List<DiffResultSpan>();
            int dcount = this._dest.Count();
            int scount = this._source.Count();

            //Deal with the special case of empty files
            if (dcount == 0)
            {
                if (scount > 0)
                {
                    retval.Add(DiffResultSpan.CreateDeleteSource(0, scount));
                }
                return retval;
            }
            else
            {
                if (scount == 0)
                {
                    retval.Add(DiffResultSpan.CreateAddDestination(0, dcount));
                    return retval;
                }
            }


            this._matchList.Sort();
            int curDest = 0;
            int curSource = 0;
            DiffResultSpan last = null;

            //Process each match record
            foreach (DiffResultSpan drs in this._matchList)
            {
                if ((!this.AddChanges(retval, curDest, drs.DestIndex, curSource, drs.SourceIndex)) && (last != null))
                {
                    last.AddLength(drs.Length);
                }
                else
                {
                    retval.Add(drs);
                }
                curDest = drs.DestIndex + drs.Length;
                curSource = drs.SourceIndex + drs.Length;
                last = drs;
            }

            //Process any tail end data
            this.AddChanges(retval, curDest, dcount, curSource, scount);

            return retval;
        }
    }
}

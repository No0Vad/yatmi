using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Yatmi.Enum;

namespace Yatmi.Entities;

public class Emotes : IEnumerable
{
    private static readonly Emotes _emptyInstance = new();

    private readonly List<EmoteEntity> _emotes;
    private string _renderedMessageAsHtml;


    public Emotes()
    {
        _emotes = new List<EmoteEntity>();
    }


    internal void Add(int startPos, int endPos, string emoteId)
    {
        _emotes.Add(new EmoteEntity
        {
            StartPos = startPos,
            EndPos = endPos,
            Lenght = endPos - startPos + 1,
            EmoteID = emoteId
        });
    }

    //public string RenderMessageAsHtml(string message)
    //{
    //	if (_emotes.Count == 0)
    //	{
    //		return message;
    //	}

    //	if (_renderedMessageAsHtml != null)
    //	{
    //		return _renderedMessageAsHtml;
    //	}

    //	for (int i = _emotes.Count - 1; i >= 0; i--)
    //	{
    //		var index = _emotes[i].StartPos;
    //		var length = _emotes[i].Lenght;
    //		message = message.Remove(index, Math.Min(length, message.Length - index)).Insert(index, $"[{_emotes[i].EmoteID}]");
    //	}

    //	_renderedMessageAsHtml = message;

    //	return _renderedMessageAsHtml;
    //}

    public string RenderMessageAsHtml(string message, EmoteSizes emoteSize = EmoteSizes.Small, string brightnessMode = "dark")
    {
        if (brightnessMode != "dark" && brightnessMode != "light")
        {
            throw new ArgumentException("Only the values light and dark are accepted!", nameof(brightnessMode));
        }

        if (_emotes.Count == 0)
        {
            return message;
        }

        if (_renderedMessageAsHtml != null)
        {
            return _renderedMessageAsHtml;
        }

        var sb = new StringBuilder();
        var span = message.AsSpan();

        // Starting text?
        if (_emotes[0].StartPos != 0)
        {
            sb.Append(span[.._emotes[0].StartPos]);
        }

        for (int i = 0; i < _emotes.Count; i++)
        {
            // According to Visual Studio, this is better?
            sb
                .Append("<img alt=\"")
                .Append(span.Slice(_emotes[i].StartPos, _emotes[i].Lenght))
                .Append("\" src=\"https://static-cdn.jtvnw.net/emoticons/v2/")
                .Append(_emotes[i].EmoteID)
                .Append("/default/")
                .Append(brightnessMode)
                .Append('/')
                .Append((int)emoteSize)
                .Append(".0\" />");

            if (i != _emotes.Count - 1)
            {
                sb.Append(span.Slice(_emotes[i].EndPos + 1, _emotes[i + 1].StartPos - _emotes[i].EndPos - 1));
            }
        }

        // Ending text?
        if (_emotes[^1].EndPos < message.Length)
        {
            sb.Append(span[(_emotes[^1].EndPos + 1)..]);
        }

        return _renderedMessageAsHtml ??= sb.ToString();
    }


    public static Emotes Parse(string emotes)
    {

        if (string.IsNullOrEmpty(emotes))
        {
            return _emptyInstance;
        }

        var emotesEntity = new Emotes();

        var span = (emotes + "/").AsSpan();
        int index;

        while ((index = span.IndexOf('/')) != -1)
        {
            var data = span[..index];

            var split = data.IndexOf(':');
            var key = data[..split];
            var val = data[(split + 1)..];

            var splitPos = val.IndexOf('-');
            var keyPos = val[..splitPos];
            var valPos = val[(splitPos + 1)..];

            span = span[(index + 1)..];

            emotesEntity.Add(
                int.Parse(keyPos.ToString()),
                int.Parse(valPos.ToString()),
                key.ToString()
            );
        }

        return emotesEntity;
    }


    public IEnumerator GetEnumerator()
    {
        return _emotes.GetEnumerator();
    }


    private class EmoteEntity
    {
        public int StartPos { get; set; }
        public int EndPos { get; set; }
        public int Lenght { get; set; }
        public string EmoteID { get; set; }
    }
}
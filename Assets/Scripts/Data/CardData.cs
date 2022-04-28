using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardRef
{
    public string CardName;
    public int Quantity;

    public CardRef(string cardName, int quantity)
    {
        CardName = cardName;
        Quantity = quantity;
    }
}

/// <summary>
/// Base class for internal interaction parameters.
/// </summary>
public abstract class InteractionData { }

public class ProduceData : InteractionData
{
    public CardRef Worker;
    public CardRef Output;
    public float Duration;

    public ProduceData(CardRef worker, CardRef output, float duration)
    {
        Worker = worker;
        Output = output;
        Duration = duration;
    }
}

/// <summary>
/// Contains interactions in their serializable form.
/// </summary>
public struct InteractionMetadata
{
    public Type InteractionType;
    public InteractionData Data;

    public InteractionMetadata(Type interactionType, InteractionData data)
    {
        InteractionType = interactionType;
        Data = data;
    }
}

/// <summary>
/// Contains cards in their serializable form.
/// </summary>
public struct CardData
{
    /// <summary>
    /// Display name of the card.
    /// </summary>
    public string Name;
    /// <summary>
    /// Card's image path relative to the JSON's folder.
    /// </summary>
    public string ImageFilePath;

    public InteractionMetadata[] Interactions;
}

namespace Content.Server.Body.Components;

/// <summary>
/// Raised when an entity is going to be gibbed, but has not
/// had any of its entities deleted or components shut down
/// </summary>
/// <param name="body"></param>
public readonly struct BeforeGibbedEvent;

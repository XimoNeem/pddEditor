using System;

public class ActionContainer
{
    public Action Action;

    public ActionContainer(Action action)
    {
        Action = action;
    }
}
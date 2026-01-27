using System;

public class InfoEventArgs<T> : EventArgs
{
    public T info;

    public InfoEventArgs()
    {
        info = default;
    }

    public InfoEventArgs(T info)
    {
        this.info = info;
    }
}

public class InfoEventArgs<T, T2> : EventArgs
{
    public T info;
    public T info2;
    public InfoEventArgs()
    {
        info = default;
    }
    public InfoEventArgs(T info, T info2)
    {
        this.info = info;
        this.info2 = info2;
    }
}
using System;

public class MsgBoxArgs : EventArgs
{
    public string title="";
    public string msg1 = "";
    public string msg2 = "";
    public string msg3 = "";
    public int [ ] btntypes = new int [ 4 ];
}
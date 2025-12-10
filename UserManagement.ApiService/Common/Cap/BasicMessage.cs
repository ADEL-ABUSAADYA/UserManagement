using System;
using System.Runtime.CompilerServices;

public class BasicMessage<T>
{
    public T Data { get; set; }
    public DateTime Date { get; set; }
    public string Sender { get; set; }
    public string Action { get; set; }

    public BasicMessage(T data, [CallerMemberName] string action = null, [CallerFilePath] string sender = null)
    {
        Data = data;
        Date = DateTime.UtcNow;
        Action = action; // اسم الميثود اللي نادى الدالة
        Sender = System.IO.Path.GetFileNameWithoutExtension(sender); // اسم الملف/الكلاس
    }
}


Thread thread = new(ActionPrint);
thread.Name = "Parallel";
thread.IsBackground = true;

thread.Start(100);

Thread.CurrentThread.Name = "Main";
ActionPrint(20);



void ActionPrint(object count)
{
    if(count is int countInt)
        for(int i = 0; i < countInt; i++)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}: {i}");
            Thread.Sleep(10);
        }
            
}
    










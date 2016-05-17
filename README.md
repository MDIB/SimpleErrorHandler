# SimpleErrorHandler

This is a simple and straightforward class helps you treat errors, exceptions.
Uses pure C#, that is why this code is not as beautiful as I wanted to be.
C# way to treat functions made it a little complicated but I've bypassed this.
If anyone wants to contribute in other languages it will be awesome.
I intend to make it in Java (as it is my mother language by now).

Some examples on how to use the class are below.

	new Handable()
            .ForValue("212")
            .ForFunction<int,string>(Convert.ToInt32)
            .OnError(() =>
            {
				Console.WriteLine("That is not an integer !!");
            })
			.OnError((ex)=>
			{
				Console.WriteLine("Some problem occurred : "+ex.Message);
			})
            .OnSucess<int>((number) =>
            {
               Console.WriteLine("That's definitely the number " + number);
            })
			.OnSucess(()=>
			{
				Console.WriteLine("Everything has gone just OK");
			})
             .Execute<int,string>() //the ugly part I've talked, everytime i have to make use of these generics....
             .Finish<int>(0); //if you provide some argument it will return it as default if some exception occur


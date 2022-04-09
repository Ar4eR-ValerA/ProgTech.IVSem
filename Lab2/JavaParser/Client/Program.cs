using Client;

var requester = new RequesterExample();
await requester.Post1(new CustomerCreateDto("dsa", "vsa"));
await requester.Get();

Console.WriteLine();
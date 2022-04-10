using Client;

var requester = new JavaMvcApplication();
var t1 = await requester.create(new CustomerCreateDto("va", "vad"));
var t2 = await requester.getAll();
var t3 = await requester.isNumber(1);
var t4 = await requester.onlyName("vav");
var t5 = await requester.getByName("va");

Console.WriteLine();
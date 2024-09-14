// See https://aka.ms/new-console-template for more information

using EventDriven;

// var topic1 = new PubSub();
// TODO: debug ReaderWriterPubSub and ReaderWriterLockSemaphore
var topic1 = new ReaderWriterPubSub();

var publisher1 = new Publisher("1");
var publisher2 = new Publisher("2");

var subscriber1 = new Subscriber("1");
var subscriber2 = new Subscriber("2");
var subscriber3 = new Subscriber("3");
topic1.Subscribe(subscriber1);
topic1.Subscribe(subscriber2);
topic1.Subscribe(subscriber3);

await publisher1.Publish(topic1, new Message("hi there"));
await publisher2.Publish(topic1, new Message("hi there as well"));
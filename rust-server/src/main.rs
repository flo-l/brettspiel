use std::any::Any;
use futures::future::{LocalBoxFuture, FutureExt};

type EventFuture<'a> = LocalBoxFuture<'a, ()>;
type EventOccur = for<'a> fn (&mut Game, data: &'a mut EventData) -> EventFuture<'a>;
type EventPossible = fn (&mut Game, data: &mut EventData) -> bool;
type EventData = dyn Any + Send;

struct Game;

struct Event {
    possible: EventPossible,
    occur: EventOccur,
    data: Box<EventData>,
}

fn always(_game: &mut Game, _data: &mut EventData) -> bool {
    true
}

fn event1<'a>(_game: &mut Game, data: &'a mut EventData) -> EventFuture<'a> {
    let s = data.downcast_mut::<String>().unwrap();
    async move {
        println!("String {}", s);
    }.boxed_local()
}

fn event2<'a>(_game: &mut Game, data: &'a mut EventData) -> EventFuture<'a> {
    let u = data.downcast_mut::<usize>().unwrap();
    async move {
        println!("usize {}", u);
    }.boxed_local()
}

fn add_event(e: &mut Event) {
    let mut game = Game;
    if (e.possible)(&mut game, &mut e.data) {
        let _f = (e.occur)(&mut game, &mut e.data);
    }
}

async fn call_event(e: &mut Event) {
    let mut game = Game;
    if (e.possible)(&mut game, &mut e.data) {
        (e.occur)(&mut game, &mut e.data).await;
    }
}


#[async_std::main]
async fn main() {
    let mut e1 = Event {
        possible: always,
        occur: event1,
        data: Box::new("String data".to_string()),
    };

    let mut e2 = Event {
        possible: always,
        occur: event2,
        data: Box::new(42),
    };

    add_event(&mut e1);
    call_event(&mut e2).await;

    let mut game = Game;
    (e1.occur)(&mut game, &mut e1.data).await;
    (e2.occur)(&mut game, &mut e2.data).await;
}

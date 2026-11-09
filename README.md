# LEE SO'S COOL BOOKS AND TOPICS MANAGEMENT APPLICATION FOR ATTRACTIVE PEOPLE

Look on my works, ye Mighty, and despair!

If you have Visual Studio, you could just open the solution and then start a debug session. Otherwise, to run the application, navigate to LSLibraryApp.Server and then run `dotnet run`. You'll see a terminal window pop up with a link to the frontend localhost page. No further setup should be necessary.

## Backend

For the API backend **LSLibraryApp.Server**, I'm using ASP.NET Core (it's a proven choice that just works) with Entity Framework Core (giving up finer control for ease and speed of development)[^1].

What you'll find in there is a fairly standard layered architecture for separations of concerns and responsibilities. Makes things neater; easier to maintain and build upon. Here's how the flow goes:

1. Controllers receive the API requests and are responsible for mapping to/from DTOs, calling services, and returning responses.
2. Services are responsible for business logic. They call repositories.
3. Repositories are responsible for data access.

[^1]: I'm doing a code-first approach to EF Core to keep things simple and fast. Many projects utilize a database-first approach to make things more explicit and controllable—for instance you could actually do things like designing the SQL tables yourself instead of trying to figure out what EF code corresponds to what SQL. Some elect to skip the ORM entirely in order to squeeze out more performance, but the current problem is nowhere near complicated enough to warrant that.

### Endpoints

With ASP.NET controllers, we can set up REST endpoints that return responses in standardized way using ActionResult.

- GET /topics
  - returns 200 OK and a list of result TopicDTOs, which may be empty.
- GET /topics/{id}
  - returns 200 OK with the result TopicDTO
  - ...or 404 Not Found.
- POST /topics
  - returns 201 Created containing the resulting TopicDTO, along with information on its location in the response header. I guess it's technically acceptable to return 200 OK, but this seems the most proper and complete.
  - If an ArgumentException is thrown (e.g., due to invalid tag data) we return a 400 Bad Request with information on what went wrong.
- PUT /topics/id
  - returns 200 OK along with the newest copy of the saved TopicDTO.
  - It can also return 404 Not Found, 400 Bad Request, or 409 Conflict (if the version is mismatched).
    - When the version is rejected, one could technically say it's a bad request (since the version is incorrect) or that it's 412 Precondition Failed (the conditons to update aren't met), but in my opinion, 409 most specifically describes the version conflict situation. We return a friendly error message in any case.

... it's a similar story with the rest of the endpoints.

Creating or deleting BookReferences returns updated information about of all of the Book's BookReferences. These operations can change the position/level values of every BookReference within the Book, and a caller would find it very useful to have the latest information about a book without having to send another query for BookReferences every single time.

### Model Relationships

In this app, Books contain BookReferences which reference Topics. To summarize the relationships between domain entities:

- A Topic can be referenced by many BookReferences.

- A Book can be referenced by many BookReferences.

- A BookReference references one Topic and one Book.

- Books and Topics don't reference eachother directly.

With EF Core, we can set up all these relationships between entities without too much trouble.

We've got Entity models that represent what's in the database, and we've got corresponding DTO objects that are used to pass information to and from layers without having to worry about making everything match the database table's shape.

I use _Mapster_ to make mapping between these models a little easier. I've also got some additional setup concerning how certain fields are mapped across in _MappingConfiguration.cs_.

#### Topic Tags

The absolute simplest thing to do would've been to store tags as some kind of serialized list of strings under a Topic, but that'll have some performance implications when you've got a ton of Topics you're wanting to e.g. retrieve by tag (which is largely the point of tags!). With EF Core, it's not too hard to do a little better than that.

The assumption is that tags will only ever be reached through topics -- that is, tags belong to topics and only exist in relation to them, full-stop.

Therefore, they're implemented as TopicTags that are an Owned Entity under a Topic, not fully-independent entities with a many-to-many relationship, which makes things somewhat simpler to manage (e.g., we don't have to worry about cleaning orphaned Tags that aren't referenced by anything). If requirements were to change such that we'd end up having to implement some kind of universal tag management system, this would no longer be fit for purpose...

### Database

To minimize setup required and get this running as soon as possible, I've settled for using an in-memory database. This is aaabsolutely not suitable for production and would be one of the first things to be replaced were this a real project. Choosing a 'real' database would also mean some additional configuration required to set up seed data and unit testing...

### BookReferences

We always upsert these in batches whenever a BookReference is added or removed. Those operations often mean the positions and levels of the other BookReferences within the Book need to be recalculated and saved.

I get the feeling some performance improvements are possible here, but I try to get something that works and is easy to understand first before Maybe trying to be clever later.

#### The BookReference Deletion and its Consequences

The easiest thing to do would've been to simply refuse to allow deleting a BookReference that is immediately followed by a BookReference of a higher level. The user experience would be miserable, though, so I've made it cope in the following way:

Every BookReference positioned after the deletion target that has a consecutively rising Level will have their Level decreased by 1. This traversal stops the moment we encounter a BookReference that doesn't have a higher level. I've sketched some scenarios out on paper and this seems like it'll work.

```
      4   4
    3   3
  2
1
A B C D E F
```

For example: if we knock out BookReference B here, we'll decrement the Levels of BookReferences C and D and end up with a valid set of BookReferences.

```
        4
    3 3
  2
1
A B C D E
```

We do this in the service layer and then save the BookReferences with their updated values. This is the most business logic in the application, so it's definitely a good target for unit tests.

### Topic Reuse Report

The way I've set the entities up using EF Core means I can use LINQ on my DbContext, digging through Books and their related BookReferences and checking the TopicIds of those BookReferences.

As a side note, there's a number of potential performance gotchas when using EF Core, since it's an abstraction that turns code into SQL. This also means that you need to know what's performant SQL _AND_ know what's performant EF Core. Improvements might be possible here? Something to research...

## Frontend

I'm using React for the frontend. It's a proven and well-supported ecosystem. That being said, this is a bit of a reckless move on my part since I've not actually used React before. (I'm going to have to learn how to use it anyway...)

The absolute fastest thing would have been to use Blazor, since it's very tightly integrated into the whole .NET ecosystem, but I'm tight enough on time that the difference in what I could get done probably isn't all that much different. Besides—React is pretty much the most popular framework out there, which means that a lot of people will know how to pick up what I've put down.

As part of the 'React and ASP.NET Core' Visual Studio project template, we've already got a build tool (Vite) and some boilerplate set up. Time is a factor, so I'm using a React component library. I want something that's opinionated and offers complete, styled components out of the box, so I won't be using something like TailwindCSS by itself. A quick search tells me many people are happy with [Mantine](https://mantine.dev), so I've went with that.

...Unfortunately, I've only managed to get the display of Topics working before I started to run out of time. Seriously not ideal!!! I'll talk more about this in a later section.

## Testing

I've got a xUnit unit test project _LSLibraryApp.Server.Tests_. Test coverage isn't totally comprehensive, but there's enough in there to cover the major bits and communicate the pattern for the rest.

The high priority test targets are:

- Validation of Topic Tags (_TopicTagsValidationTests.cs_)
- Updating a Topic with the wrong Version throws the appropriate error (_TopicRepositoryTests.cs_)
- Adding BookReferences (_BookServiceTests.cs_)
  - Level must not be more than one deeper than the previously-positioned BookReference
  - Positions are updated as expected
- Deleting BookReferences (_BookServiceTests.cs_)
  - Levels are updated as expected

Also, ASP.NET projects in Visual Studio make manual testing API endpoints easy with .http files (e.g., _LSLibraryApp.Server.http_). Just open them up in the IDE and you can send requests. **This is how I've been manually testing my API.**

## Hey so what about deleting topics

The trouble is that any number of BookReferences could point to a Topic. There's a few potential ways to deal with that...

1. Don't let them do it! (unless there's no references)

This means a user would have to painstakingly hunt down and deal with every single reference before they could delete a topic. If they've got a very large number of references to a topic, they'd have to torture themselves quite a bit in order to manage this.

2. Just let them do it!

Extremely dangerous!!! This would mean the deletion of any number of BookReferences across any number of Books, which could cause utter havoc. Even if we were to clearly communicate the consequences to the user beforehand, even thinking about making this possible makes me nervous.

I prefer safety. Allowing a careless mistake to cause extensive damage to a user's data isn't great. To make sure the user experience is no more painful than it needs to be, I think the ideal solution would be to have topic deletion be a more involved and interactive process where the user is presented with related BookReferences that they can manage through a friendly UI before deleting a topic.

## Unfinished work, regrets, failures, sorrows

Boy, where do I start,

### Frontend

It's just incomplete. Sorry, chief, but I'm sticking to the prescribed time limit. I probably could have gotten more work done quicker had I ordered a clanker servant to do my bidding, but I'm not made of money, and I doubt you're wanting to assess how many LLM service credits I'm willing to purchase anyway.[^2]

[^2]: AI tools have been used to carry out some research, occasionally help debug, and generate a few unit tests in _TopicRepositoryTests.cs_ and _TopicTagsValidationTests.cs_. All in all, they weren't used that much.

#### What Could Have Been

What's currently in there is a table that lists Topics. Each row has an 'Actions' column containing an 'Edit' icon button. The first order of business would have been to hook that up to a Mantine Modal component where the user could edit the topic.

The upserting modal component would receive a DTO object (either from the list, or a new one in case of a create), use that to populate some form controls, and would submit a POST or PUT depending on whether the DTO had an Id. There's a possibility that edit operations could conflict since they're using data from the table—we could handle Conflict responses by prompting to reload data into the modal (re-GET the entity). If we were expecting Topics to get updated frequently (and therefore cause frequent version conflicts) we could alleviate that problem by always working with a freshly-retrieved copy of the data.

Responses from the server would have been used to patch client-side data immediately without requiring another network round-trip to fetch the latest data.

Another table (perhaps displayed in its own tab) would have handled Books in a similar way.

The UI would have been split into components across separate files (e.g., TopicsTable, TopicUpsertModal, BooksTable, BookUpsertModal...).

### Other improvements and topics for consideration

1. The frontend! It'd be nice to actually finish it! Ugh!
2. That Topic table's just sitting in App.tsx instead of being inside its own component. Needs more organization.
3. DELETE {bookId}/references/{bookReferenceId} doesn't actually use bookId at all, because the bookReferenceId is enough to uniquely identify a BookReference. I guess we could try to check if the BookReference does really belong to the Book? Probably needlessly strict?
4. Incomplete unit test coverage. Need to cover more cases...
5. When you update a Topic or add a BookReference you provide both TopicId/BookId and a DTO that also contains TopicId/BookId. I've chosen to do strict validation where we ensure both Ids match. Wonder if that's the best course of action...
6. We do version conflict checking for Topics, but not for anything else!!! Also it's technically possible that Topics could no longer be updated once their version number reaches the maximum int, but surely there's no way that'll happen in real use lmao
7. We've got some validation logic in the service layer, and we've got some validation logic in model attributes (e.g., TopicTagsValidationAttribute). I wonder if there's a better way to organize this and keep it more centralized?
8. We've got plain-jane string literals on display in the frontend. Ideally all display strings would be passed through a localizer.
9. There might be a more performant way to implement some of the business logic, maybe the way we handle BookReference positions and levels...
10. You can't actually delete Books yet. The service and repository methods are already there, they're just not hooked up to a controller endpoint. The intention is for a Book deletion to also delete all content (i.e., associated BookReferences). See _LibraryDbContext.cs_, specifically the .OnDelete().
11. The business logic DeleteBookReferenceFromBook() should be one atomic operation. It deletes and then upserts—if the upsert somehow fails it should roll back the delete...
   
I'll stop talking now. See ya!

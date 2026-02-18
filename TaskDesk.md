Task: Restaurants Review
Est. time/effort:
30 hours

Task Description
Expected delivery time: 7 days



Task Description
Restaurants review platform API

We would like to see how you would create an API for a simplified restaurant review platform.



Task scope and functional requirements
Your task is to  build a REST or GraphQL API for a restaurant review platform.
Authentication/Authorization:

Users should be able to sign up. Each user can be of “reviewer” or “owner” type.
APIs available to all users:

Restaurant listing API.
Listing needs to have pagination.
The listing needs to be filterable by partially matching the restaurant titles.
The listing needs to be filterable by coordinates + radius. This is to allow client applications to present to users “restaurants in a 5-mile radius from me”.
Reviews API
Ability to list all reviews. Paginated, filterable by restaurant ID.
Ability to create a review for a restaurant (Reviewer user only).
Every review should have the following attributes:
Review text.
Date created.
Restaurant reference.
Rating (1-5).
Recently viewed restaurants API (Reviewer user only).
Needs to provide functionality to record restaurants as being viewed.
Needs to provide the functionality to list the last 10 viewed restaurants.
Restaurants API (Owner only)
Owners need to be able to create/update/delete restaurants.
Owners can only manage restaurants they created.
Every restaurant should have:
Title
Preview image
Coordinates
Description
Reviews
Average rating (calculated based on reviews).
Coordinates field is required but can be passed as an “Address” when creating/updating a restaurant. For example, the request can contain an “address” field with a value of “Alexanderplatz, Berlin, Germany”. The backend should create a restaurant with the correct coordinates.


Technical requirements
API has to be GraphQL or RESTful.
Be mindful of the edge cases and unexpected scenarios.
Be mindful of security in general, both authentication and authorization.
Be mindful about validating input and handling input errors gracefully.


Milestones and task delivery
The deadline to submit your completed project is 1 week from the moment you receive the project requirements.
It means the project code must be submitted within 1 week from the moment that was delivered to you by email.
If you schedule your final interview after the 1-week deadline, make sure to submit your completed project and all code to the private repository before the deadline.
Everything that is submitted after the deadline will not be taken into consideration.
To ensure sufficient review time, please commit all code or documents at least six hours before the meeting. Submissions after this time will not be considered.
Please join the meeting room for this final interview on time. If you miss your interview without prior notice, your application may be paused for six months.
# TrackTV_WEB
Track.tv is a mockup of a movie database. The app runs on top of remote Oracle Database with most of the domain logic implemented in stored procedures. An ASP.NET web application is used as a client, with a custom ORM used to provide data consistency.

Screenshots:

![Alt text](TrackTV_WEB/blob/master/profile.png?raw=true "Profile page with personalised statistics")

![Alt text](TrackTV_WEB/blob/master/addshow.png?raw=true "Interface for manipulating with the movies/tv shows database")

See [project_analysis.pdf](TrackTV_WEB/blob/master/project_analysis.pdf)  for a comprehensive analysis of of the project.
See [procedures.pdf](TrackTV_WEB/blob/master/procedures.pdf) for implementation of the pl/sql procedures.

Only two views from the user interface are implemented. Connection to the database also works only when connected to the network of VŠB-TUO, since the server is currently stored there.

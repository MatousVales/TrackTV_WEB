create or replace procedure ActorAwards(showID IN Shows."sID"%type)
as 
BEGIN
savepoint svpt1;
  for onerecord in (SELECT Actors."aID" as ID, Actors.Name 
                    FROM Actors
                    JOIN StarsIn ON Actors."aID" = StarsIn.Actors_aID
                    JOIN Shows ON StarsIn.Shows_sID = Shows."sID"
                    WHERE Shows."sID" = showID)
  LOOP
   UPDATE Actors SET Actors.AmountOfAwards = Actors.AmountOfAwards + 1 WHERE Actors."aID" = onerecord.ID;
  END LOOP;
  
  UPDATE Shows SET Shows.hasGoldenGlobe = 1 WHERE Shows."sID" = showID;
  
  EXCEPTION WHEN OTHERS THEN
  ROLLBACK TO svpt1;
END;

------------------------------------------------


create or replace procedure ActorsAverage
as 
BEGIN
savepoint svpt1;
  for onerecord in (SELECT AVG(Rating.Percentage) as average, Actors.Name as ActorName
                    FROM Rating
                    JOIN Shows ON Shows."sID" = Rating.shows_sID
                    JOIN StarsIn ON StarsIn.Shows_sID = Shows."sID"
                    JOIN Actors ON Actors."aID" = StarsIn.Actors_aID
                    GROUP BY Actors.Name)
  LOOP
   UPDATE Actors SET Actors.average = onerecord.average WHERE Actors.Name = onerecord.ActorName;
  END LOOP;
  
  EXCEPTION WHEN OTHERS THEN
  ROLLBACK TO svpt1;
END;

------------------------------------------------

create or replace procedure AddUser(fName IN Users.Name%type,
                                    pwd IN Users.Password%type,
                                    isuseradmin IN Users.isAdmin%type,
                                    userID OUT Users."uID"%type
                                  )
as
  counter INT;
  login Users.Login%type;
begin
  SELECT COUNT(Users.Name) INTO counter FROM Users GROUP BY Users.Name HAVING Users.name = fName;
  counter:= counter + 1;
  login := substr(fName, 0, 3) || TO_CHAR(counter);
  INSERT INTO Users (Login,Password,isAdmin,Name) VALUES(login,pwd,isuseradmin,fName) RETURNING "uID" INTO userID;
  
  EXCEPTION
  WHEN NO_DATA_FOUND THEN
    counter := 0;
  counter:= counter + 1;
  login := substr(fName, 0, 3) || TO_CHAR(counter);
  INSERT INTO Users (Login,Password,isAdmin,Name) VALUES(login,pwd,isuseradmin,fName) RETURNING "uID" INTO userID;
end;


-------------------------------------------------------------


create or replace procedure DailyScore(
                                        score OUT INT,
                                        userLogin OUT Users.Login%type
                                       )
as 
  suma INT := -1;
  counter INT := -1;
  currentID Users."uID"%type;
BEGIN

savepoint svpt1;

  for onerecord in (SELECT * FROM USERS)
  LOOP
  BEGIN
    currentID := onerecord."uID";
    suma := -1;
    counter := -1;
    
    SELECT SUM(Comments.Score) INTO suma FROM Comments GROUP BY Comments.Users_uID, TRUNC(Comments.Datetime) HAVING Comments.Users_uID = onerecord."uID" AND TRUNC(Comments.Datetime) = TRUNC(SYSDATE);
    SELECT COUNT(*) INTO counter FROM History GROUP BY History.Users_uID, TRUNC(History.Datetime) HAVING History.Users_uID = onerecord."uID" AND TRUNC(History.Datetime) = TRUNC(SYSDATE);
    suma:= suma + (counter*10);
    UPDATE Users SET Users.DailyScore = suma WHERE Users."uID" = onerecord."uID";
    
     EXCEPTION
      WHEN NO_DATA_FOUND THEN
      if (suma = -1) then
        suma := 0;
      end if;
      if (counter = -1) then
        counter := 0;
      end if;
      suma:= suma + (counter*10);
      UPDATE Users SET Users.DailyScore = suma WHERE Users."uID" = onerecord."uID";
      CONTINUE;
  END;
  
  --------------------------------------------
  
  create or replace PROCEDURE RateShow(userID IN Rating.Users_uID%type,
                                     showID IN Rating.Shows_sID%type,
                                     percentage IN Rating.Percentage%type,
                                     outcome OUT varchar2)
as 
average INT;
result varchar2(20);
BEGIN
  BEGIN
    INSERT INTO Rating VALUES (userID,showID,percentage);
  END;
  SELECT AVG(Rating.Percentage) into average FROM Rating GROUP BY Shows_sID HAVING Shows_sID = showID;
  dbms_output.put_line(average);
    if (average <= 25) then
		  UPDATE Shows SET Shows.Stars =  1 WHERE Shows."sID" = showID;
	  ELSIF (average <= 50) then
		  UPDATE Shows SET Shows.Stars = 2 WHERE Shows."sID" = showID;
	  ELSIF (average <= 75) then
		  UPDATE Shows SET Shows.Stars = 3 WHERE Shows."sID"= showID;
	  ELSIF (average < 100) then
		  UPDATE Shows SET Shows.Stars = 4 WHERE Shows."sID" = showID;
	  ELSIF (average = 100) then
		  UPDATE Shows SET Shows.Stars = 5 WHERE Shows."sID" = showID;
	  end if;
    ACTORSAVERAGE();
    result:= 'SUCCESS';
    outcome:= result;
    
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
    result := 'FAIL';
    outcome:= result;
END;

----------------------------------------------------------------
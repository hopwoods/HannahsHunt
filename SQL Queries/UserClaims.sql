SELECT U.UserName, C.ClaimType, C.ClaimValue
FROM dbo.AspNetUsers AS U
INNER JOIN dbo.AspNetUserClaims AS C ON U.Id = C.UserId
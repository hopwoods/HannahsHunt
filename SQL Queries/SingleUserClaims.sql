SELECT U.UserName, C.ClaimType, C.ClaimValue
FROM dbo.AspNetUsers AS U
INNER JOIN dbo.AspNetUserClaims AS C ON U.Id = C.UserId
WHERE U.Id = 'f3cedeb6-a223-4760-87f1-86aaa6f04d84'
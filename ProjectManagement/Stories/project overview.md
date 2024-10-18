# TownTalk Project Overview

## Project Name
**TownTalk** -  A local news platform where citizens can share news, interact with each other, and stay informed about their community.

## Key Features
1. **User Registration and Authentication**
   - Utilize ASP.NET Identity for managing user accounts.

2. **News Posts**
   - Users can create, edit, and delete news posts.
   - Each post has a title, content, creation date, and associated user.

3. **Comments**
   - Users can comment on posts.
   - Each comment has content, creation date, and associated user and post.

4. **Likes/Dislikes**
   - Users can react to posts with likes or dislikes.

5. **Notifications**
   - Users receive notifications about relevant actions (e.g., new comments, reactions).

6. **Categories**
   - Posts can be categorized for easier browsing.

7. **User Follow System**
   - Users can follow other users to receive updates on their posts.

## Models
1. **ApplicationUser**
   - Extends `IdentityUser` to include additional properties (e.g., `DisplayName`, `DateJoined`).

2. **Post**
   - Properties: `Id`, `Title`, `Content`, `CreatedAt`, `UserId`, `Comments`, `Reactions`, `CategoryId`.

3. **Comment**
   - Properties: `Id`, `Content`, `CreatedAt`, `PostId`, `UserId`.

4. **Reaction**
   - Properties: `Id`, `UserId`, `PostId`, `IsLike`.

5. **Notification**
   - Properties: `Id`, `UserId`, `Message`, `IsRead`, `CreatedAt`.

6. **Category**
   - Properties: `Id`, `Name`, `Posts`.

7. **UserFollow**
   - Properties: `Id`, `FollowerId`, `FollowedId`.

## DbContext
- **TownTalkDbContext**
   - Contains DbSet properties for all models.
   - Configures relationships and composite keys.

## Steps to Setup
1. Create a new MVC project with .NET 6.
2. Add necessary NuGet packages for EF Core and Identity.
3. Create and configure models.
4. Update the DbContext.
5. Create and apply migrations.

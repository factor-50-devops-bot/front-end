# front-end

Website to be hosted at helpmystreet.org

## Configuration

`appsettings.json` has default local configurations. To override set the following environment variables:

| Env var                       | Description                                 |
| ----------------------------- | ------------------------------------------- |
| `Firebase__CredentialKeyFile` | Absolute location of the firebase cred file |
| `Services__Address__Location` | URL of the Address service                  |
| `Services__Address__Key`      | Auth key for the Address service            |
| `Services__User__Location`    | URL of the User service                     |
| `Services__User__Key`         | Auth key for the User service               |

## HTML Build

The frontend uses Webpack to build static sources in to minified version and places them in the `wwwroot` folder of the MVC application

`npm run build` will build the sources

`npm run watch` will watch the sources and rebuild on changes.

## ToDo:

- [ ] Run `npm run build` when building the main project for release
- [ ] Split CSS out of Webpack bundle and include as separate file

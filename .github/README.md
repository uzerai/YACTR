```
                              __   __ _______ _______ _______  ______
                                \_/   |_____| |          |    |_____/
                                 |    |     | |_____     |    |    \_
```
<p align="center"><strong>Y</strong>et <strong>A</strong>nother <strong>C</strong>limbing <strong>TR</strong>acker</p>

<p align="center">
  <a href="#why">Objective</a> •
  <a href="#features">Features</a> •
  <a href="#development">Development</a> •
  <a href="#contributing">Contributing</a>
</p>

> This is currently WIP and full documentation for usage and 
installation are yet to be complete or maintained.

----

 **YACTR** is a climbing tracker whose intention is to simplify the collection of crags, ascents, bolting/maintenance organizations and climbing community specific information.

## Objective
The final objective of the software in this repository is to be able to serve an single or multi-instance, cloneable, API container which can be easily set up and managed by anyone, for free, to track climbing information with a focus on community management through a granular permission system.

This in stark contrast to a subsection of climbing tracking apps and sites who only serve their subscription based users the complete set of information about the sport.

The need for this arose when I realized there's no plug-and-play, user-friendly solution for a problem I _believe_ smaller climbing groups face when setting up their own routes.
Either going public on the larger route aggregation websited (theCrag / 27Crags / 8a.nu / etc.), and forfeiting administrative rights over crags they themselves maintain; or building a solution through no-code or low-code platforms which usually ends up unmaintained and feature-incomplete (which this project likely will mimic one part of).


## Features
> WIP (documentation, not features)

## Development

### Requirements
- Dotnet 10.0
    - Was it the right choice for this project? Likely not,
    but it's what I was building and RBAC system in anyways.
- Docker
    - Most development relies on the dockerization of subservices, and as such it's best to have it installed. The output of our releases is also a docker image (moving to open-image standard at some point i guess).
- [github.com/casey/just](https://github.com/casey/just) 
    - for running of justfile commands (see the justfile), makes development a lot easier instead of having to fuck around with `dotnet` sdk commands I always forget _some_ option to.
- 

## Contributing
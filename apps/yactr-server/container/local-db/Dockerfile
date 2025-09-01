FROM postgres:17.4 AS builder

RUN apt-get update && apt-get install -y \
    build-essential \
    git \
    curl \
    tar \
    postgresql-server-dev-17 \
    postgresql-17-postgis-3 \
    postgis \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir "/tmp/pg_uuidv7"  
WORKDIR "/tmp/pg_uuidv7"
RUN curl -LO "https://github.com/fboulnois/pg_uuidv7/releases/download/v1.6.0/{pg_uuidv7.tar.gz,SHA256SUMS}" && \
    tar xf pg_uuidv7.tar.gz && \
    sha256sum -c SHA256SUMS

RUN cp "$PG_MAJOR/pg_uuidv7.so" "$(pg_config --pkglibdir)" && \
    cp pg_uuidv7--1.6.sql pg_uuidv7.control "$(pg_config --sharedir)/extension"

RUN rm -rf /tmp

FROM builder AS runner

COPY container/local-db/init-scripts /docker-entrypoint-initdb.d

EXPOSE 5432
CMD ["postgres"]

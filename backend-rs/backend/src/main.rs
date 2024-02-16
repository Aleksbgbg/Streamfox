mod config;
mod controllers;
mod models;

use crate::config::{Config, ConfigError};
use crate::controllers::user;
use crate::models::migrations::migrator::Migrator;
use axum::{routing, Router};
use sea_orm::{Database, DatabaseConnection, DbErr};
use sea_orm_migration::MigratorTrait;
use snowflake::SnowflakeIdBucket;
use std::io;
use std::net::SocketAddr;
use std::sync::Arc;
use thiserror::Error;
use tokio::net::TcpListener;
use tokio::sync::Mutex;
use tower_http::trace::{DefaultMakeSpan, DefaultOnResponse, TraceLayer};
use tracing::{info, Level};

#[derive(Error, Debug)]
enum AppError {
  #[error("could not load config")]
  LoadConfig(#[from] ConfigError),
  #[error("could not connect to database")]
  ConnectToDatabase(DbErr),
  #[error("could not run database migrations")]
  MigrateDatabase(DbErr),
  #[error("could not bind to network interface")]
  BindTcpListener(io::Error),
  #[error("could not get TCP listener address")]
  GetListenerAddress(io::Error),
  #[error("could not start Axum server")]
  ServeApp(io::Error),
}

#[derive(Clone)]
pub struct AppState {
  config: Arc<Config>,
  connection: DatabaseConnection,
  snowflakes: Arc<Snowflakes>,
}

pub struct Snowflakes {
  pub user_snowflake: Mutex<SnowflakeIdBucket>,
}

#[tokio::main]
async fn main() -> Result<(), AppError> {
  let config = Arc::new(config::load()?);

  tracing_subscriber::fmt()
    .with_target(false)
    .compact()
    .init();

  let connection = Database::connect(&config.database.connection_string())
    .await
    .map_err(AppError::ConnectToDatabase)?;
  Migrator::up(&connection, None)
    .await
    .map_err(AppError::MigrateDatabase)?;

  let listener = TcpListener::bind(SocketAddr::from((config.app.host, config.app.port)))
    .await
    .map_err(AppError::BindTcpListener)?;

  let app = Router::new()
    .route("/auth/register", routing::post(user::register))
    .route("/auth/login", routing::post(user::login))
    .route("/user", routing::get(user::get_user))
    .layer(
      TraceLayer::new_for_http()
        .make_span_with(DefaultMakeSpan::new().level(Level::INFO))
        .on_response(DefaultOnResponse::new().level(Level::INFO)),
    )
    .with_state(AppState {
      config,
      connection,
      snowflakes: Arc::new(Snowflakes {
        user_snowflake: Mutex::new(SnowflakeIdBucket::new(1, 1)),
      }),
    });
  let api = Router::new().nest("/api", app);

  info!(
    "backend available at: {}",
    listener
      .local_addr()
      .map_err(AppError::GetListenerAddress)?
  );

  axum::serve(listener, api)
    .await
    .map_err(AppError::ServeApp)?;

  Ok(())
}
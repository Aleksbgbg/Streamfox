use chrono::Duration;
use serde::{Deserialize, Serialize};
use serde_with::{serde_as, DurationSeconds};
use thiserror::Error;
use toml_env::Args;

type Ipv4Array = [u8; 4];
type Port = u16;

fn to_string(array: &Ipv4Array) -> String {
  array.map(|x| x.to_string()).join(".")
}

#[derive(Serialize, Deserialize)]
pub struct Config {
  pub app: App,
  pub database: Database,
}

#[serde_as]
#[derive(Serialize, Deserialize)]
pub struct App {
  pub config_root: String,
  pub host: Ipv4Array,
  pub port: Port,
  #[serde_as(as = "DurationSeconds<i64>")]
  pub token_lifespan: Duration,
}

#[derive(Serialize, Deserialize)]
pub struct Database {
  pub host: Ipv4Array,
  pub port: Port,
  pub name: String,
  pub user: String,
  pub password: String,
}

impl Database {
  pub fn connection_string(&self) -> String {
    format!(
      "postgres://{}:{}@{}:{}/{}",
      self.user,
      self.password,
      to_string(&self.host),
      self.port,
      self.name,
    )
  }
}

#[derive(Error, Debug)]
pub enum ConfigError {
  #[error("could not load config")]
  Load(#[from] toml_env::Error),
  #[error("config was not found")]
  NotFound,
}

pub fn load() -> Result<Config, ConfigError> {
  toml_env::initialize(Args {
    config_variable_name: "config",
    ..Default::default()
  })?
  .ok_or(ConfigError::NotFound)
}

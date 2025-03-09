using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "budget",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_sync_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    available = table.Column<long>(type: "bigint", nullable: false, defaultValue: 10000000L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    maintenance_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    background_jobs_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "domain",
                columns: table => new
                {
                    host = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    app = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain", x => x.host);
                });

            migrationBuilder.CreateTable(
                name: "global_task",
                columns: table => new
                {
                    key = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    requirements = table.Column<string>(type: "jsonb", nullable: false),
                    payout = table.Column<long>(type: "bigint", nullable: false),
                    wagering_multiplier = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    profile_id = table.Column<int>(type: "integer", nullable: true),
                    security_id = table.Column<Guid>(type: "uuid", nullable: true),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    referral_campaign_id = table.Column<Guid>(type: "uuid", nullable: true),
                    @virtual = table.Column<bool>(name: "virtual", type: "boolean", nullable: false, defaultValue: false),
                    roles = table.Column<short[]>(type: "smallint[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    provider_user_id = table.Column<string>(type: "text", nullable: false),
                    provider_username = table.Column<string>(type: "text", nullable: true),
                    provider_user_first_name = table.Column<string>(type: "text", nullable: false),
                    provider_user_last_name = table.Column<string>(type: "text", nullable: true),
                    provider_user_image = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "global_task_status",
                columns: table => new
                {
                    task_key = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_global_task_status_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    kind = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    auto_close = table.Column<bool>(type: "boolean", nullable: false),
                    auto_close_ms = table.Column<int>(type: "integer", nullable: false),
                    with_close_button = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    has_custom_name = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    used_provider = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_profile_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promocode",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false),
                    wagering_multiplier = table.Column<int>(type: "integer", nullable: false),
                    usages = table.Column<int>(type: "integer", nullable: false),
                    max_usages = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    bonus = table.Column<string>(type: "jsonb", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    campaign = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    bonus_type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promocode", x => x.id);
                    table.ForeignKey(
                        name: "FK_promocode_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_promocode_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referral_campaign",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    total_visits = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_signups = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_campaign", x => x.id);
                    table.ForeignKey(
                        name: "FK_referral_campaign_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referrer_balance",
                columns: table => new
                {
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrer_balance", x => x.referrer_id);
                    table.ForeignKey(
                        name: "FK_referrer_balance_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referrer_payout",
                columns: table => new
                {
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    next_payout_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_payout_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrer_payout", x => x.referrer_id);
                    table.ForeignKey(
                        name: "FK_referrer_payout_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referrer_settings",
                columns: table => new
                {
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rev_share = table.Column<int>(type: "integer", nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrer_settings", x => x.referrer_id);
                    table.ForeignKey(
                        name: "FK_referrer_settings_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referrer_withdrawal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrer_withdrawal", x => x.id);
                    table.ForeignKey(
                        name: "FK_referrer_withdrawal_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    game_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    game = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_security",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_ip = table.Column<string>(type: "text", nullable: true),
                    address_usdt = table.Column<string>(type: "text", nullable: true),
                    address_usdc = table.Column<string>(type: "text", nullable: true),
                    address_trx = table.Column<string>(type: "text", nullable: true),
                    address_eth = table.Column<string>(type: "text", nullable: true),
                    address_btc = table.Column<string>(type: "text", nullable: true),
                    address_ltc = table.Column<string>(type: "text", nullable: true),
                    address_ton = table.Column<string>(type: "text", nullable: true),
                    address_doge = table.Column<string>(type: "text", nullable: true),
                    address_bnb = table.Column<string>(type: "text", nullable: true),
                    address_xmr = table.Column<string>(type: "text", nullable: true),
                    address_sol = table.Column<string>(type: "text", nullable: true),
                    whitelisted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ip_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_ton_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_usdt_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_usdc_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_trx_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_eth_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_btc_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_ltc_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_doge_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_bnb_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_xmr_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_sol_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_security", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_security_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deposit_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    withdraw_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_stats_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    text = table.Column<string>(type: "text", nullable: true),
                    attachments = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'[]'::jsonb"),
                    sender_name = table.Column<string>(type: "text", nullable: true),
                    sender_username = table.Column<string>(type: "text", nullable: true),
                    sender_image = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    profile_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    sender_roles = table.Column<short[]>(type: "smallint[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_message", x => x.id);
                    table.ForeignKey(
                        name: "FK_chat_message_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chat_message_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promocode_usage",
                columns: table => new
                {
                    promocode_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promocode_usage", x => new { x.promocode_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_promocode_usage_promocode_promocode_id",
                        column: x => x.promocode_id,
                        principalTable: "promocode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promocode_usage_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referrer_transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    referral_id = table.Column<Guid>(type: "uuid", nullable: true),
                    referral_campaign_id = table.Column<Guid>(type: "uuid", nullable: true),
                    referral_action = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referrer_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_referrer_transaction_referral_campaign_referral_campaign_id",
                        column: x => x.referral_campaign_id,
                        principalTable: "referral_campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_referrer_transaction_user_referral_id",
                        column: x => x.referral_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_referrer_transaction_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    referrer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    referral_campaign_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_referral_campaign_referral_campaign_id",
                        column: x => x.referral_campaign_id,
                        principalTable: "referral_campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_session_user_referrer_id",
                        column: x => x.referrer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_session_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    gem_amount = table.Column<long>(type: "bigint", nullable: false),
                    currency_amount = table.Column<decimal>(type: "numeric(20,18)", precision: 20, scale: 18, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_transaction_id = table.Column<string>(type: "text", nullable: false),
                    provider_amount = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    currency = table.Column<short>(type: "smallint", nullable: false),
                    method = table.Column<short>(type: "smallint", nullable: false),
                    provider = table.Column<short>(type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit", x => x.id);
                    table.ForeignKey(
                        name: "FK_deposit_transaction_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deposit_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    game = table.Column<short>(type: "smallint", nullable: false),
                    outcome = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    bet = table.Column<long>(type: "bigint", nullable: false),
                    multiplier = table.Column<int>(type: "integer", nullable: false),
                    payout = table.Column<long>(type: "bigint", nullable: false),
                    preview_user_name = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_record_transaction_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_record_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "withdrawal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v7()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    gem_amount = table.Column<long>(type: "bigint", nullable: false),
                    currency_amount = table.Column<decimal>(type: "numeric(20,18)", precision: 20, scale: 18, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_transaction_id = table.Column<string>(type: "text", nullable: false),
                    provider_amount = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    currency = table.Column<short>(type: "smallint", nullable: false),
                    method = table.Column<short>(type: "smallint", nullable: false),
                    provider = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawal", x => x.id);
                    table.ForeignKey(
                        name: "FK_withdrawal_transaction_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_withdrawal_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "balance",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    wagering_required = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_bet = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_win = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_lost = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_bet_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    max_win = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    max_multiplier = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    max_win_game_id = table.Column<Guid>(type: "uuid", nullable: true),
                    max_multiplier_game_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balance", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_balance_game_record_max_multiplier_game_id",
                        column: x => x.max_multiplier_game_id,
                        principalTable: "game_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_balance_game_record_max_win_game_id",
                        column: x => x.max_win_game_id,
                        principalTable: "game_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_balance_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "budget",
                columns: new[] { "id", "available", "created_at", "last_sync_at", "updated_at" },
                values: new object[] { 1, 10000000L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "config",
                columns: new[] { "id", "background_jobs_enabled", "created_at", "updated_at" },
                values: new object[] { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_account_provider_user_id",
                table: "account",
                column: "provider_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_account_user_id",
                table: "account",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_balance_max_multiplier_game_id",
                table: "balance",
                column: "max_multiplier_game_id");

            migrationBuilder.CreateIndex(
                name: "IX_balance_max_win_game_id",
                table: "balance",
                column: "max_win_game_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_message_is_pinned",
                table: "chat_message",
                column: "is_pinned",
                filter: "(\"is_pinned\" = true)");

            migrationBuilder.CreateIndex(
                name: "IX_chat_message_profile_id",
                table: "chat_message",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_message_user_id",
                table: "chat_message",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_transaction_id",
                table: "deposit",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_user_id",
                table: "deposit",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_record_transaction_id",
                table: "game_record",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_record_user_id",
                table: "game_record",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_global_task_status_user_id",
                table: "global_task_status",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_user_id",
                table: "profile",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profile_username",
                table: "profile",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_promocode_code",
                table: "promocode",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_promocode_created_by",
                table: "promocode",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_promocode_expires_at",
                table: "promocode",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_promocode_user_id",
                table: "promocode",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_promocode_usage_user_id",
                table: "promocode_usage",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_referral_campaign_code",
                table: "referral_campaign",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_campaign_referrer_id",
                table: "referral_campaign",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_referrer_transaction_created_at",
                table: "referrer_transaction",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_referrer_transaction_referral_campaign_id",
                table: "referrer_transaction",
                column: "referral_campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_referrer_transaction_referral_id",
                table: "referrer_transaction",
                column: "referral_id");

            migrationBuilder.CreateIndex(
                name: "IX_referrer_transaction_referrer_id",
                table: "referrer_transaction",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_referrer_withdrawal_referrer_id",
                table: "referrer_withdrawal",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_referral_campaign_id",
                table: "session",
                column: "referral_campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_referrer_id",
                table: "session",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_user_id",
                table: "session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_created_at",
                table: "transaction",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_user_id",
                table: "transaction",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_referrer_id",
                table: "user",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_security_last_ip",
                table: "user_security",
                column: "last_ip");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_transaction_id",
                table: "withdrawal",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_user_id",
                table: "withdrawal",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "balance");

            migrationBuilder.DropTable(
                name: "budget");

            migrationBuilder.DropTable(
                name: "chat_message");

            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "deposit");

            migrationBuilder.DropTable(
                name: "domain");

            migrationBuilder.DropTable(
                name: "global_task");

            migrationBuilder.DropTable(
                name: "global_task_status");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "promocode_usage");

            migrationBuilder.DropTable(
                name: "referrer_balance");

            migrationBuilder.DropTable(
                name: "referrer_payout");

            migrationBuilder.DropTable(
                name: "referrer_settings");

            migrationBuilder.DropTable(
                name: "referrer_transaction");

            migrationBuilder.DropTable(
                name: "referrer_withdrawal");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "user_security");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "withdrawal");

            migrationBuilder.DropTable(
                name: "game_record");

            migrationBuilder.DropTable(
                name: "profile");

            migrationBuilder.DropTable(
                name: "promocode");

            migrationBuilder.DropTable(
                name: "referral_campaign");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}

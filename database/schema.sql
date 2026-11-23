-- Review
create table public.reviews (
  id          bigint generated always as identity primary key,
  author      text not null check (char_length(trim(author)) between 1 and 60),
  tour        text not null,
  rating      int  not null check (rating between 1 and 5),
  comment     text not null check (char_length(comment) between 10 and 500),
  deleted_at  timestamptz,                         -- soft delete
  created_at  timestamptz not null default now()
);

-- habit 1: the list page filters on deleted_at and sorts by newest
create index reviews_active_newest_idx
  on public.reviews (created_at desc)
  where deleted_at is null;

grant select, insert, update, delete on public.reviews to anon;   -- lock 1
alter table public.reviews enable row level security;             -- lock 2
create policy "read_all"   on public.reviews for select to anon using (true);
create policy "insert_all" on public.reviews for insert to anon with check (true);
create policy "update_all" on public.reviews for update to anon using (true) with check (true);
create policy "delete_all" on public.reviews for delete to anon using (true);

-- Bookings
create table public.bookings (
  id            bigint generated always as identity primary key,
  reference     text   not null unique,
  name          text   not null check (char_length(trim(name)) between 1 and 60),
  email         text   not null check (email like '%@%'),
  tour          text   not null,
  tour_date     date   not null,
  guests        int    not null check (guests between 1 and 12),
  cancelled_at  timestamptz,                        -- soft delete: a cancelled booking stays on record
  created_at    timestamptz not null default now()
);

-- fast capacity lookup: one tour on one date, active bookings only
create index bookings_tour_date_idx
  on public.bookings (tour, tour_date)
  where cancelled_at is null;

grant select, insert, update, delete on public.bookings to anon;   -- lock 1
alter table public.bookings enable row level security;             -- lock 2
create policy "read_all"   on public.bookings for select to anon using (true);
create policy "insert_all" on public.bookings for insert to anon with check (true);
create policy "update_all" on public.bookings for update to anon using (true) with check (true);
create policy "delete_all" on public.bookings for delete to anon using (true);